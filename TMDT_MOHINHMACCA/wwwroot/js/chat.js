"use strict";

$(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    connection.start().then(function () {
        console.log('SignalR started...');
        viewModel.getMessageUnread();
        viewModel.roomList();
        console.log("ahii" + viewModel.messageUnread());
    }).catch(function (err) {
        return console.error(err);
    });
    connection.on("newMessage", function (messageView) {
        var ismine = (messageView.senderId === viewModel.myProfile().username());
        var message = new ChatMessage(messageView.messageId,
            messageView.senderId,
            messageView.receiverId,
            replaceUrlsWithLinks(messageView.content),
            messageView.status,
            ismine,
            messageView.senttime
        );
        if (messageView.receiverId === viewModel.myProfile().username()) {
            viewModel.chatMessages.push(message);
            /*$(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);*/
            viewModel.getMessageUnread();
        }

    });
    connection.on("getProfileInfo", function (user) {
        viewModel.myProfile(new ProfileInfo(user.username, user.fullname, user.avatar));
        viewModel.isLoading(false);
    });

    function ProfileInfo(username, fullname, avatar) {
        var self = this;
        self.username = ko.observable(username);
        self.fullname = ko.observable(fullname);
        self.avatar = ko.observable(avatar);
    }
    function ChatRoom(username, fullname, avatar, lastmess, lastdate, unread, online, device) {
        var self = this;
        self.username = ko.observable(username);
        self.fullname = ko.observable(fullname);
        self.avatar = ko.observable(avatar);
        self.unread = ko.observable(unread);
        self.lastmess = ko.observable(lastmess);
        self.lastdate = ko.observable(lastdate);
        self.online = ko.observable(online);
        self.device = ko.observable(device);
        self.updateUnread = function () {
            self.lastdate(0);
        }
        self.timestampRelative = ko.pureComputed(function () {
            var date = new Date(self.lastdate());
            var now = new Date();
            var diff = Math.round((date.getTime() - now.getTime()) / (1000 * 3600 * 24));
            var { dateOnly, timeOnly } = formatDate(date);
            if (diff == 0)
                return `Today, ${timeOnly}`;
            if (diff == -1)
                return `Yestrday, ${timeOnly}`;

            return `${dateOnly}`;
        });
        self.timestampFull = ko.pureComputed(function () {
            var date = new Date(self.lastdate());
            var { fullDateTime } = formatDate(date);
            return fullDateTime;
        });
    }
    function ChatMessage(id, senderid, recerverid, content, status, ismine, senttime, avatarsender) {
        var self = this;
        self.id = ko.observable(id);
        self.content = ko.observable(content);
        self.status = ko.observable(status);
        self.senttime = ko.observable(senttime);
        self.senderid = ko.observable(senderid);
        self.recerverid = ko.observable(recerverid);
        self.avatarsender = ko.observable(avatarsender);
        self.ismine = ko.observable(ismine);
        self.timestampRelative = ko.pureComputed(function () {
            // Get diff
            var date = new Date(self.senttime());
            var now = new Date();
            var diff = Math.round((date.getTime() - now.getTime()) / (1000 * 3600 * 24));

            // Format date
            var { dateOnly, timeOnly } = formatDate(date);

            // Generate relative datetime
            if (diff == 0)
                return `Today, ${timeOnly}`;
            if (diff == -1)
                return `Yestrday, ${timeOnly}`;

            return `${dateOnly}`;
        });
        self.timestampFull = ko.pureComputed(function () {
            var date = new Date(self.senttime());
            var { fullDateTime } = formatDate(date);
            return fullDateTime;
        });
    }

    function formatDate(date) {
        // Get fields
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var d = hours >= 12 ? "PM" : "AM";

        // Correction
        if (hours > 12)
            hours = hours % 12;

        if (minutes < 10)
            minutes = "0" + minutes;

        // Result
        var dateOnly = `${day}/${month}/${year}`;
        var timeOnly = `${hours}:${minutes} ${d}`;
        var fullDateTime = `${dateOnly} ${timeOnly}`;

        return { dateOnly, timeOnly, fullDateTime };
    }
    function AppViewModel() {
        var self = this;

        self.joinedRoom = ko.observable();
        self.message = ko.observable("");
        self.Users = ko.observableArray([]);
        self.chatRooms = ko.observableArray([]);
        self.chatMessages = ko.observableArray([]);
        self.myProfile = ko.observable();
        self.isLoading = ko.observable(true);
        self.filter = ko.observable("");
        self.search = ko.observable("");
        self.messageUnread = ko.observable("");
        self.fullName = ko.computed(function () {
            return self.chatMessages();
        }, this);
        self.showAvatar = ko.computed(function () {
            return self.isLoading() == false && self.myProfile().avatar() != null;
        });
        self.onlineStatus = ko.computed(function () {
            var room = self.joinedRoom();
            return room && room.online && room.online() === true ? "Online" : "Offline";
        }, self);
        self.uploadFiles = function () {
            var form = document.getElementById("uploadForm");
            $.ajax({
                type: "POST",
                url: '/api/Upload',
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function () {
                    $("#UploadedFile").val("");
                    self.roomList();
                    self.joinRoom(self.chatRooms()[0]);
                    self.search("");
                },
                error: function (error) {
                    alert('Error: ' + error.responseText);
                }
            });

        }
        self.joinRoom = function (room) {
            self.joinedRoom(room);
            self.messageHistory();
        }
        self.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.sendNewMessage();
            }
            return true;
        }
        self.showAvatar = ko.computed(function () {
            return self.isLoading() == false && self.myProfile().avatar() != null;
        });
        self.sendNewMessage = function () {
            self.sendToUser(self.joinedRoom().username(), self.message());
            self.message("");
        }
        self.getMessageUnread = function () {
            fetch('/api/MessageAPI/getUnread')
                .then(response => response.json())
                .then(data => {
                    self.messageUnread(data + "");
                    console.log(self.messageUnread());
                });
        }
        self.sendToUser = function (userid, message) {
            if (userid.length > 0 && message.length > 0) {
                fetch('/api/MessageAPI/Create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ ReceiverId: userid, Content: message })
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error('Failed to create message.');
                        }
                        self.roomList();
                        self.joinRoom(self.chatRooms()[0]);
                        self.search("");

                    })
                    .catch(error => {
                        console.error('Error sending message:', error);
                        // Xử lý lỗi (hiển thị thông báo cho người dùng, v.v.)
                    });
            } else {
                console.error('User ID or message content is invalid.');
                // Xử lý lỗi (hiển thị thông báo cho người dùng, v.v.)
            }
        }
        self.roomList = function () {

            fetch('/api/MessageAPI/chatRoom')
                .then(response => response.json())
                .then(data => {
                    self.chatRooms.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        self.chatRooms.push(new ChatRoom(data[i].username, data[i].fullname, data[i].avatar, truncateText(data[i].lastmess, 20), data[i].lastdate, data[i].unread,data[i].online, data[i].device));
                    }

                });
        }
        self.debouncedSearch = ko.pureComputed(self.search).extend({
            rateLimit: { method: 'notifyWhenChangesStop', timeout: 300 }
        });

        // Hàm tìm kiếm người dùng
        self.usersSearch = function () {
            console.log("search:", self.search());
            fetch('api/MessageAPI/getUsers/' + self.search())
                .then(response => response.json())
                .then(data => {
                    self.Users.removeAll(); // Xóa tất cả các phần tử trong Users
                    data.forEach(user => {
                        self.Users.push(new ProfileInfo(user.username, user.fullname, user.avatar));
                    });
                })
                .catch(error => {
                    console.error('Error fetching data:', error);
                    self.Users.removeAll(); // Xóa tất cả các phần tử trong Users nếu có lỗi
                });
        };

        // Theo dõi sự thay đổi của debouncedSearch
        self.debouncedSearch.subscribe(function (newValue) {
            console.log("New value of search:", newValue);
            self.usersSearch(); // Gọi usersSearch khi search observable thay đổi (sau khi ngừng nhập)
        });
        self.messageHistory = function () {
            fetch('/api/MessageAPI/chatUser/' + viewModel.joinedRoom().username())
                .then(response => response.json())
                .then(data => {
                    self.chatMessages.removeAll();
                    console.log(data[0])
                    for (var i = 0; i < data.length; i++) {
                        var ismine = (data[i].senderId === self.myProfile().username());
                        self.chatMessages.push(new ChatMessage(data[i].messageId,
                            data[i].senderId,
                            data[i].receiverId, replaceUrlsWithLinks(data[i].content),
                            data[i].status,
                            ismine,
                            data[i].senttime,
                            data[i].avatarsender
                        ));
                    }
                    scrollToBottom();
                });
            self.getMessageUnread();
            self.chatRooms().forEach(function (item) {
                if (item.username() === self.joinedRoom().username()) {
                    item.unread(0);
                }
            });

        }

    }
    ko.computed(function () {
        // Bất cứ khi nào chatMessages thay đổi (vd: được tải từ máy chủ)
        scrollToBottom();
    });
    function truncateText(text, maxLength) {
        if (text.length > maxLength) {
            return text.substring(0, maxLength) + '...';
        } else {
            return text;
        }
    }
    function replaceUrlsWithLinks(message) {
        // Biểu thức chính quy để tìm kiếm các đường dẫn URL trong văn bản
        var urlRegex = /(https?:\/\/[^\s]+)/g;

        // Thay thế các đường dẫn URL bằng thẻ <a>
        var replacedMessage = message.replace(urlRegex, function (url) {
            return '<a href="' + url + '" target="_blank">' + url + '</a>';
        });

        return replacedMessage;
    }

    function scrollToBottom() {
        var container = $(".messages-container");
        container.scrollTop(container.prop("scrollHeight"));
    }
    var viewModel = new AppViewModel();
    ko.applyBindings(viewModel);
});