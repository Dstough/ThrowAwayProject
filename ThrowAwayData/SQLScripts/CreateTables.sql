--SQLite doesn't enforce the length of a varchar
--you can put varchar and it will let you
--insert 500+ characters into it.

--INTEGER PRIMARY KEY is an alias for Row Id
--in sqlite. This is the equivilant to SQL server's
--Identity field

create table if not exists UserGroup
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	Deleted bit not null,

	Name varchar not null,
	Description varchar not null,
    Style varchar null
);

create table if not exists UserIdentity
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED,
	Deleted bit not null,

	UserGroupId integer not null references UserGroup(Id) DEFERRABLE INITIALLY DEFERRED,
	Email varchar not null,
	UserName varchar not null,
	Style varchar,
	Passphrase varchar not null,
	VerificationCode varchar not null,
	Authenticated bit not null,
	Banned bit not null,
	Dead bit not null
);

alter table UserGroup ADD CreatedBy references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED;

create table if not exists Tag
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED,
	Deleted bit not null,

	Name varchar not null
);

create table if not exists Thread
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED,
	Deleted bit not null,

	Title varchar not null,
	Body varchar,
	TagId integer not null references Tag(Id) DEFERRABLE INITIALLY DEFERRED,
	Edited bit not null,
	Closed bit not null
);

create table if not exists Post
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null unique,
	CreatedOn datetime not null,
	CreatedBy integer not null references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED,
	Deleted bit not null,

	Body varchar not null,
	Edited bit not null,
	ThreadId integer not null references Thread(Id) DEFERRABLE INITIALLY DEFERRED
);

create table if not exists Article
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null,
	CreatedOn datetime not null,
	CreatedBy integer not null references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED,
	Deleted bit not null,

	Title varchar not null,
	Body varchar not null
);

create table if not exists Comment
(
	Id INTEGER PRIMARY KEY,
	PublicId varchar not null,
	CreatedOn datetime not null,
	CreatedBy integer not null references UserIdentity(Id) DEFERRABLE INITIALLY DEFERRED,
	Deleted bit not null,

	Edited bit not null,
	Body varchar not null
);

begin;

	insert into UserGroup(PublicId, CreatedOn, CreatedBy, Deleted, Name, Description, Style)
	values
		('Admin', datetime('now'), 1, 0, 'Admin', 'The primary group giving you access', 'color: #ff0;'),
		('Runner', datetime('now'), 1, 0, 'Runner', 'The group for people with above normal access','color: #fff;'),
		('User', datetime('now'), 1, 0, 'User', 'The primary group for users to post', ' ')
		('Chummer', datetime('now'), 1, 0, 'Chummer', 'The first group all accounts become', 'color: #aaa');

	insert into Tag(PublicId, CreatedOn, CreatedBy, Deleted, Name)
	values
		('User', datetime('now'), 1, 0, 'User'),
		('News', datetime('now'), 1, 0, 'News'),
		('Jobs', datetime('now'), 1, 0, 'Jobs'),
		('Runs', datetime('now'), 1, 0, 'Runs');

	insert into UserIdentity(PublicId, CreatedOn, CreatedBy, Deleted, UserGroupId, Email, UserName, Style, Passphrase, VerificationCode, Authenticated, Banned, Dead)
	values 
		('F@$T_J@CK', datetime('now'), 1, 0, 1, 'fastjack@jackpoint.ddns.net', 'Fastjack', 'color: #ff0', 'fastjack', 'fastjack-jackpoint', 1, 0, 0),
		('D3U$', datetime('now'), 1, 0, 1, 'Deus@jackpoint.ddns.net', 'Deus', 'color: #ff0', 'fastjack', 'Deus0ajskhb1in321aa3p3nt', 1, 0, 0);

commit;