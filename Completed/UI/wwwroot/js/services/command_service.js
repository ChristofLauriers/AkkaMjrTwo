(function() {
    "use strict";

    var module = angular.module('dice-game');

    module.service('commandService', function($http) {


        var baseUrl = "http://localhost:5001/api/";

        return {
            createGame: function(success, error) {
            
                $http.post(baseUrl + 'game/create')
                    .then(function(response) {
                        success(response);
                    }, function(resp){
                        error();
                    });
            },
            startGame: function(gameId, playersCount, error) {
                var players = []
                for (i = 0; i < playersCount; i++) { 
                    players[i] = 'player' + i;
                }

                $http.post(baseUrl + '/game/start', { GameId : gameId, Players: players })
                     .error(error);
            },
            roll: function(gameId, player, error) {
                $http.post(baseUrl + '/game/roll', { GameId : gameId, Player: player })
                     .error(error);
            }
        };
    });

})();