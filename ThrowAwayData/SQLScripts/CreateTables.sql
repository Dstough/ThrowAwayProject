--SQLite doesn't enforce the length of a varchar
--you can put varchar and it will let you
--insert 500+ characters into it.

--INTEGER PRIMARY KEY is an alias for Row Id
--in sqlite. This is the equivilant to SQL server's
--Identity field

create table UserGroup
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	Name varchar not null,
	Description varhcar not null
);

insert into UserGroup(PublicId, CreatedOn, CreatedBy, Deleted, Name, Description)
values
    ('Admin', datetime('now'), 0, 0, 'Admin', 'The primary group giving you access'),
    ('Runner', datetime('now'), 0, 0, 'Runner', 'The group for people who can post on closed topics'),
    ('Users', datetime('now'), 0, 0, 'User', 'The primary group for users to post');

create table Tag
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	Name varchar not null
);

insert into Tag(PublicId, CreatedOn, CreatedBy, Deleted, Name)
values
    ('User', datetime('now'), 0, 0, 'User'),
    ('News', datetime('now'), 0, 0, 'News'),
    ('Jobs', datetime('now'), 0, 0, 'Jobs'),
    ('Runs', datetime('now'), 0, 0, 'Runs');

create table UserIdentity
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	UserGroupId integer not null,
	Email varchar,
	UserName varchar not null,
	Passphrase varchar not null,
	VerificationCode varchar not null,
	Authenticated bit not null,
	Banned bit not null,
	Dead bit not null
);

create table Thread
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	Body varchar not null,
	TagId integer,
	Edited bit not null,
	Closed bit not null,
	constraint fk_Tag
		foreign key (TagId)
		references Tag(Id)
);

create table Post
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	ThreadId integer,
    Section varchar,
	Body varchar not null,
	Edited bit not null,
	constraint fk_Thread
		foreign key (ThreadId)
		references Thread(Id)
);