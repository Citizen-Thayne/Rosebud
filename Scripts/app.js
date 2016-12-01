var app = angular.module('RoseBud', ['ngResource', 'ui.bootstrap']);


app.factory('MovieList', function ($resource) {
    return $resource('/api/MovieLists/:Id/', null, {
        "addMovie": {
            method: "POST",
            url: "/api/MovieLists/:Id/AddMovie"
        },
        "removeMovie": {
            method: "DELETE",
            url: "/api/MovieLists/:Id/RemoveMovie/:movieId"
        },
        "update": {
            method: "PUT"
        }
    });
})

app.factory('MovieSearch', function ($resource) {
    return $resource('/api/Movies/search');
})

app.controller('MovieListsController', ['MovieList', '$scope', function (MovieList, $scope) {
    $scope.movieLists = MovieList.query();

    $scope.showNewListModal = function () {
        $scope.newModalName = null;
        $('#new-list-modal').modal('show');
    }

    $scope.submitNewModal = function (name) {
        if (name.length > 0) {
            list = new MovieList();
            list.Name = name;
            list.$save(function () {
                $scope.movieLists = MovieList.query();
            });
            $('#new-list-modal').modal('hide');
        }
    }

    $scope.$on('reloadMovieLists', function (event, data) {
        $scope.movieLists = MovieList.query();
    })

}])

app.controller('MovieListController', function ($scope, MovieList) {
    $scope.active = false;
    $scope.listEdit = false;

    $scope.toggleActive = function (list) {
        $scope.active = !$scope.active;
        $('#list-' + list.Id).collapse('toggle')
    }

    $scope.editMovieList = function () {
        if ($scope.listEdit) {
            $scope.list.$update({ Id: $scope.list.Id }, $scope.list);
        }

        $scope.listEdit = !$scope.listEdit;
    }

    $scope.removeFromList = function (list, movie) {
        MovieList.removeMovie({ Id: list.Id, movieId: movie.ID }, function (data) {
            $scope.list = data
        })
    }

    $scope.deleteMovieList = function (list) {
        MovieList.remove({ id: list.Id }, function () {
            $scope.$emit('reloadMovieLists')
        });
    }
    $scope.$on('movieAddedToList', function (event, data) {
        $scope.list = data.list;
    })
})


app.controller('MovieSearchController', function ($scope, MovieSearch, MovieList) {
    $scope.$watch("searchText", function (newValue, oldValue) {
        if ($scope.searchText.length > 0) {
            MovieSearch.get({ s: $scope.searchText }, function (data) {
                $scope.results = data;
            })
        }
    })
    $scope.movieLists = MovieList.query();
    $scope.addToList = function (list, movie) {
        MovieList.addMovie({ Id: list.Id }, movie, function (updatedList) {
            $scope.$emit('movieAddedToList', { movie: movie, list: updatedList })
        })
    }
})
