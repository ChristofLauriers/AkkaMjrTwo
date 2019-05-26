(function() {
    "use strict";
    
    var module = angular.module('dice-game');

    module.service('eventService', function($rootScope) {
        var connection = null;
        return {
            connect: function(gameId) {

                connection = new signalR.HubConnectionBuilder()
                                        .withUrl('/hub/event')
                                        .build();

                connection.on('broadcastEvent', function (message) {
                    $rootScope.$broadcast('events.' + message.eventType, message.event);
                });

                connection.start()
                    .then(function () {
                        console.log('connection started');
                        connection.invoke('AddToGroup', $rootScope.gameId);
                    })
                    .catch(error => {
                        console.error(error.message);
                    });
            },
            disconnect: function() {
                if (connection != null) {
                    connection.invoke('RemoveFromGroup', $rootScope.gameId);
                    connection.stop();
                }
            }
        };
    });

})();
