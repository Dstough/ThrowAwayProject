drop table if exists UserGroup;
drop table if exists Tag;
drop table if exists UserIdentity;
drop table if exists Thread;
drop table if exists Post;
create table if not exists UserGroup (
  Id INTEGER NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (Id),
  PublicId varchar(11) not null,
  CONSTRAINT uniquePublicId UNIQUE (PublicId),
  CreatedOn datetime not null,
  CreatedBy integer not null,
  Deleted bit not null,
  Name varchar(30) not null,
  Description varchar(300) not null
);
insert into UserGroup(
    PublicId,
    CreatedOn,
    CreatedBy,
    Deleted,
    Name,
    Description
  )
values
  (
    'Admin',
    now(),
    0,
    0,
    'Admin',
    'The primary group giving you access'
  ),
  (
    'Runner',
    now(),
    0,
    0,
    'Runner',
    'The group for people who can post on closed topics'
  ),
  (
    'Users',
    now(),
    0,
    0,
    'User',
    'The primary group for users to post'
  );
create table if not exists Tag (
    Id INTEGER not null AUTO_INCREMENT,
    PRIMARY KEY(Id),
    PublicId varchar(11) not null,
    CONSTRAINT uniquePublicId UNIQUE (PublicId),
    CreatedOn datetime not null,
    CreatedBy integer not null,
    Deleted bit not null,
    Name varchar(50) not null
  );
insert into Tag(PublicId, CreatedOn, CreatedBy, Deleted, Name)
values
  ('User', now(), 0, 0, 'User'),
  ('News', now(), 0, 0, 'News'),
  ('Jobs', now(), 0, 0, 'Jobs'),
  ('Runs', now(), 0, 0, 'Runs');
create table if not exists UserIdentity (
    Id INTEGER not null AUTO_INCREMENT,
    PRIMARY KEY(Id),
    PublicId varchar(11) not null,
    CONSTRAINT uniquePublicId UNIQUE (PublicId),
    CreatedOn datetime not null,
    CreatedBy integer not null,
    Deleted bit not null,
    UserGroupId integer not null,
    Email varchar(100),
    UserName varchar(100) not null,
    Passphrase varchar(100) not null,
    VerificationCode varchar(36) not null,
    Authenticated bit not null,
    Banned bit not null,
    Dead bit not null
  );
create table if not exists Thread (
    Id INTEGER NOT NULL AUTO_INCREMENT,
    PRIMARY KEY(Id),
    PublicId varchar(11) not null,
    CONSTRAINT uniquePublicId UNIQUE (PublicId),
    CreatedOn datetime not null,
    CreatedBy integer not null,
    Deleted bit not null,
    Body text(65535) not null,
    TagId integer,
    Edited bit not null,
    Closed bit not null,
    constraint fk_Tag foreign key (TagId) references Tag(Id)
  );
create table if not exists Post (
    Id INTEGER NOT NULL AUTO_INCREMENT,
    PRIMARY KEY(Id),
    PublicId varchar(11) not null,
    CONSTRAINT uniquePublicId UNIQUE (PublicId),
    CreatedOn datetime not null,
    CreatedBy integer not null,
    Deleted bit not null,
    ThreadId integer,
    Section varchar(10),
    Body text(65535) not null,
    Edited bit not null,
    constraint fk_Thread foreign key (ThreadId) references Thread(Id)
  );