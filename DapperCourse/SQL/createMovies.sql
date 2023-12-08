create database if not exists movies;

create table Actors
(
    ActorId   int auto_increment
        primary key,
    FirstName longtext   not null,
    LastName  longtext   not null,
    Gender    varchar(1) not null
)
    engine = InnoDB;

create table Directors
(
    DirectorId int auto_increment
        primary key,
    FirstName  longtext not null,
    LastName   longtext not null
)
    engine = InnoDB;

create table Genres
(
    GenreId int auto_increment
        primary key,
    Title   longtext not null
)
    engine = InnoDB;

create table Movies
(
    MovieId            int auto_increment
        primary key,
    Title              longtext    not null,
    Year               int         not null,
    Duration           int         not null,
    Language           longtext    not null,
    ReleaseDate        datetime(6) null,
    ReleaseCountryCode longtext    not null
)
    engine = InnoDB;

create table DirectorMovie
(
    DirectorsDirectorId int not null,
    MoviesMovieId       int not null,
    primary key (DirectorsDirectorId, MoviesMovieId),
    constraint FK_DirectorMovie_Directors_DirectorsDirectorId
        foreign key (DirectorsDirectorId) references Directors (DirectorId)
            on delete cascade,
    constraint FK_DirectorMovie_Movies_MoviesMovieId
        foreign key (MoviesMovieId) references Movies (MovieId)
            on delete cascade
)
    engine = InnoDB;

create index IX_DirectorMovie_MoviesMovieId
    on DirectorMovie (MoviesMovieId);

create table GenreMovie
(
    GenresGenreId int not null,
    MoviesMovieId int not null,
    primary key (GenresGenreId, MoviesMovieId),
    constraint FK_GenreMovie_Genres_GenresGenreId
        foreign key (GenresGenreId) references Genres (GenreId)
            on delete cascade,
    constraint FK_GenreMovie_Movies_MoviesMovieId
        foreign key (MoviesMovieId) references Movies (MovieId)
            on delete cascade
)
    engine = InnoDB;

create index IX_GenreMovie_MoviesMovieId
    on GenreMovie (MoviesMovieId);

create table MovieCasts
(
    ActorId int      not null,
    MovieId int      not null,
    Role    longtext not null,
    primary key (ActorId, MovieId),
    constraint FK_MovieCasts_Actors_ActorId
        foreign key (ActorId) references Actors (ActorId)
            on delete cascade,
    constraint FK_MovieCasts_Movies_MovieId
        foreign key (MovieId) references Movies (MovieId)
            on delete cascade
)
    engine = InnoDB;

create index IX_MovieCasts_MovieId
    on MovieCasts (MovieId);

create table Reviewers
(
    ReviewerId int auto_increment
        primary key,
    Name       longtext null
)
    engine = InnoDB;

create table Ratings
(
    MovieId         int            not null,
    ReviewerId      int            not null,
    Stars           decimal(18, 2) null,
    NumberOfRatings int            null,
    primary key (MovieId, ReviewerId),
    constraint FK_Ratings_Movies_MovieId
        foreign key (MovieId) references Movies (MovieId)
            on delete cascade,
    constraint FK_Ratings_Reviewers_ReviewerId
        foreign key (ReviewerId) references Reviewers (ReviewerId)
            on delete cascade
)
    engine = InnoDB;

create index IX_Ratings_ReviewerId
    on Ratings (ReviewerId);

