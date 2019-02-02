--SQLite doesn't enforce the length of a varchar
--you can put varchar and it will let you
--insert 500+ characters into it.

create table Car(
	Id int primary key,
	CreatedOn DateTime Not null,
	CreatedBy Varchar not null,
	Deleted bit not null,
	Make varchar,
	Model varchar
);

create table Train(
	Id int primary key,
	CreatedOn DateTime Not null,
	CreatedBy varchar not null,
	Deleted bit not null,
	Name varchar,
	Cargo varchar,
	Cars int
);

create table Plane(
	Id int primary key,
	CreatedOn DateTime Not null,
	CreatedBy varchar not null,
	Deleted bit not null,
	Name varchar,
	Cabin varchar,
	MaxPassengerCount int
);