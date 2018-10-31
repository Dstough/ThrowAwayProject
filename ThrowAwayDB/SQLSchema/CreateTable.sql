create table Car(
	Id int Identity(1,1),
	CreatedOn DateTime Not null,
	CreatedBy Varchar(20) not null,
	Deleted bit not null,
	Make varchar(max),
	Model varchar(max)
);

create table Train(
	Id int Identity(1,1),
	CreatedOn DateTime Not null,
	CreatedBy Varchar(20) not null,
	Deleted bit not null,
	Name varchar(max),
	Cargo varchar(max),
	Cars int,
);

create table Plane(
	Id int Identity(1,1),
	CreatedOn DateTime Not null,
	CreatedBy Varchar(20) not null,
	Deleted bit not null,
	Name varchar(max),
	Cabin varchar(max),
	MaxPassengerCount int,
);