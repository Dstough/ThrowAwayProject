use JackPoint;
set foreign_key_checks = 0;

drop table if exists UserGroup;
drop table if exists UserIdentity;
drop table if exists Tag;
drop table if exists Thread;
drop table if exists Post;
drop table if exists Article;
drop event if exists MaintainData;


create table if not exists UserGroup 
(
  Id int not null AUTO_INCREMENT PRIMARY KEY,
  PublicId varchar(11) unique not null,
  CreatedOn datetime not null,
  CreatedBy int not null,
  Deleted boolean not null,

  Name varchar(30) not null,
  Description varchar(300) not null,
  Style text,

  constraint foreign key(CreatedBy)
    references UserIdentity(Id)
);

create table if not exists UserIdentity 
(
  Id int not null AUTO_INCREMENT primary key,
  PublicId varchar(11) unique not null,
  CreatedOn datetime not null,
  CreatedBy int not null,
  Deleted boolean not null,

  UserGroupId int not null,
  Email varchar(100) not null,
  UserName varchar(100) not null,
  Style text,
  Passphrase text not null,
  VerificationCode varchar(36) not null,
  Authenticated boolean not null,
  Banned boolean not null,
  Dead boolean not null,

  constraint foreign key(CreatedBy)
    references UserIdentity(Id),

  constraint foreign key(UserGroupId)
    references UserGroup(Id)
);

create table if not exists Tag 
(
  Id int not null AUTO_INCREMENT PRIMARY KEY,
  PublicId varchar(11) unique not null,
  CreatedOn datetime not null,
  CreatedBy int not null,
  Deleted boolean not null,

  Name varchar(50) not null,

  constraint foreign key(CreatedBy)
    references UserIdentity(Id)
);

create table if not exists Thread 
(
  Id int not null AUTO_INCREMENT primary key,
  PublicId varchar(11) unique not null,
  CreatedOn datetime not null,
  CreatedBy int not null,
  Deleted boolean not null,

  Title text not null,
  Body text not null,
  Edited boolean not null,
  Closed boolean not null,
  TagId int not null,

  constraint foreign key(CreatedBy)
    references UserIdentity(Id),

  constraint foreign key(TagId)
    references Tag(Id)
);

create table if not exists Post 
(
  Id int not null AUTO_INCREMENT primary key,
  PublicId varchar(11) unique not null,
  CreatedOn datetime not null,
  CreatedBy int not null,
  Deleted boolean not null,

  Body text not null,
  Edited boolean not null,
  ThreadId int not null,

  constraint foreign key(CreatedBy)
    references UserIdentity(Id),

  constraint foreign key(ThreadId)
    references Thread(Id)
);

create table if not exists Article
(
  Id int not null auto_increment primary key,
  PublicId varchar(11) unique not null,
  CreatedOn datetime not null,
  CreatedBy int not null,
  Deleted boolean not null,

  Title text not null,
  Body text not null,
  TagId int not null,

  constraint foreign key(CreatedBy)
    references UserIdentity(Id),

  constraint foreign key(TagId)
    references Tag(Id)
);

insert into UserGroup(PublicId, CreatedOn, CreatedBy, Deleted, Name, Description, Style)
values
  ('Admin',   now(), 1, FALSE, 'Admin',  'The primary group giving you access',           'color: #ff0;'),
  ('Runner',  now(), 1, FALSE, 'Runner', 'The group for people with above normal access', 'color: #fff;'),
  ('User',    now(), 1, FALSE, 'User',   'The primary group for users to post',           ' '),
  ('Chummer', now(), 1, FALSE, 'Chummer','The first group all accounts become',           'color: #aaa');

insert into UserIdentity(PublicId, CreatedOn, CreatedBy, Deleted, UserGroupId, Email, UserName, Style, Passphrase, VerificationCode, Authenticated, Banned, Dead)
values 
  ('F@$T_J@CK', now(), 1, FALSE, 1, 'fastjack@jackpoint.ddns.net', 'Fastjack', 'color: #ff0', 'fastjack', 'fastjack-jackpoint'      , TRUE, FALSE, FALSE),
  ('D3U$'     , now(), 1, FALSE, 1, 'Deus@jackpoint.ddns.net'    , 'Deus'    , 'color: #ff0', 'fastjack', 'Deus0ajskhb1in321aa3p3nt', TRUE, FALSE, FALSE);

insert into Tag(PublicId, CreatedOn, CreatedBy, Deleted, Name)
values
  ('User', now(), 1, FALSE, 'User'),
  ('News', now(), 1, FALSE, 'News'),
  ('Jobs', now(), 1, FALSE, 'Jobs'),
  ('Runs', now(), 1, FALSE, 'Runs');

create event if not exists MaintainData
on schedule every 5 minute
do begin
  
  update Thread
  set Closed = TRUE
  where CreatedOn < now() - interval 1 week;

  update UserIdentity
  set UserGroupId = 3
  where UserGroupId = 4
  and CreatedOn < now() - interval 1 week;

end;

set foreign_key_checks = 1;