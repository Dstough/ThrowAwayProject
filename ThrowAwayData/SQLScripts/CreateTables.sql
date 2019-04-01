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
	CreatedBy varchar not null,
	Deleted bit not null,
	Name varchar not null,
	Description varhcar not null
);
create table UserIdentity
(
	Id INTEGER PRIMARY KEY,
	CreatedOn datetime not null,
	CreatedBy varchar not null,
	Deleted bit not null,
	GroupId integer not null,
	Email varchar,
	UserName varchar not null,
	PassPhrase varchar not null,
	VerificationCode varchar not null,
	Authenticated bit not null,
	Banned bit not null,
	Dead bit not null
);
insert into UserGroup (CreatedOn,CreatedBy,Deleted,Name,Description)
values(datetime('now'), 'System', 0, 'Admin', 'The primary group giving you access');
insert into UserGroup (CreatedOn,CreatedBy,Deleted,Name,Description)
values(datetime('now'), 'System', 0, 'Guest', 'The group used for a default user who cannot post');
insert into UserGroup (CreatedOn,CreatedBy,Deleted,Name,Description)
values(datetime('now'), 'System', 0, 'User', 'The primary group for users to post');