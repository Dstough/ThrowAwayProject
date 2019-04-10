--SQLite doesn't enforce the length of a varchar
--you can put varchar and it will let you
--insert 500+ characters into it.

--INTEGER PRIMARY KEY is an alias for Row Id
--in sqlite. This is the equivilant to SQL server's
--Identity field
create table UserGroup
(
	Id INTEGER PRIMARY KEY,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	Name varchar not null,
	Description varhcar not null
);
create table UserIdentity
(
	Id INTEGER PRIMARY KEY,
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
create table Post
(
	Id INTEGER PRIMARY KEY,
	CreatedOn datetime not null,
	CreatedBy integer not null,
	Deleted bit not null,
	ParentId integer,
	Title varchar,
	Body varchar not null,
	Tags varchar,
	Closed bit not null
);
insert into UserGroup
	(CreatedOn,CreatedBy,Deleted,Name,Description)
values(datetime('now'), 0, 0, 'Admin', 'The primary group giving you access');
insert into UserGroup
	(CreatedOn,CreatedBy,Deleted,Name,Description)
values(datetime('now'), 0, 0, 'SuperUser', 'The group for people who can post on closed topics');
insert into UserGroup
	(CreatedOn,CreatedBy,Deleted,Name,Description)
values(datetime('now'), 0, 0, 'User', 'The primary group for users to post');