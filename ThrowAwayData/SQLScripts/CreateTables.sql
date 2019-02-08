--SQLite doesn't enforce the length of a varchar
--you can put varchar and it will let you
--insert 500+ characters into it.

--INTEGER PRIMARY KEY is an alias for Row Id
--in sqlite. This is the equivilant to SQL server's
--Identity field

create table UserGroup
(
	Id integer primary key,
	CreatedOn datetime not null,
	CreatedBy varchar not null,
	Deleted bit not null,
	Name varchar not null,
	Description varhcar not null
);

create table UserIdentity
(
	Id integer primary key,
	CreatedOn datetime not null,
	CreatedBy varchar not null,
	Deleted bit not null,
	GroupId integer not null,
	Email varchar,
	UserName varchar not null,
	PassPhrase varchar not null,
	foreign key(GroupId) references UserGroup(Id)
);

create table FileCategory
(
	Id integer primary key,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	Name varchar not null,
	Description varchar,
	foreign key(CreatedBy) references UserIdentity(Id)
)

create table FileObject
(
	Id integer primary key,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	Category int not null,
	Name varchar not null,
	Extention varchar not null,
	Bytes blob not null,
	foreign key(CreatedBy) references UserIdentity(Id),
	foreign key(Category) references FileCategory(Id)
);

insert into UserGroup
values(datetime('now'), 'System', 0, 'Admin', 'The primary group giving you access ');