DROP DATABASE IF EXISTS VeteransMuseum;

CREATE DATABASE VeteransMuseum;
USE VeteransMuseum;

CREATE TABLE Cemeteries(
CName varchar(250) not null,
CAddress varchar(100),
CCity varchar(250) not null,
GPS varchar(250),
CemAirPicLoc varchar(250),
DirectionsPicLoc varchar(250),
primary key (CName, CCity)
);

CREATE TABLE BranchesList(
BranchName varchar(250),
BranchPicLoc varchar(250),
primary key (BranchName)
);

CREATE TABLE RanksList(
BranchName varchar(250),
RankName varchar(250),
RankAbrev varchar(250),
primary key (BranchName, RankName),
foreign key (BranchName) references BranchesList (BranchName) ON UPDATE CASCADE
);

CREATE TABLE ConflictList(
CLName varchar(250),
primary key (CLName)
);

CREATE TABLE AwardsList(
BranchName varchar(250),
AwardName varchar(250),
primary key (BranchName, AwardName),
foreign key (BranchName) references BranchesList (BranchName) ON UPDATE CASCADE
);

CREATE TABLE Veterans(
ID integer not null auto_increment,
FName varchar(250),
MName varchar(250),
LName varchar(250) not null,
Suffix varchar(250),
DOB date,
DOD date,
CName varchar(250),
CCity varchar(250),
CSection varchar(250),
CRow varchar(250),
MarkerLocation varchar(250),
MarkerPicLoc varchar(250),
MilPicLoc varchar(250),
CasualPicLoc varchar(250),
MiscPicLoc varchar(250),
Comments varchar(3000),
primary key (ID),
foreign key (CName, CCity) references Cemeteries (CName, CCity) ON UPDATE CASCADE
);

CREATE TABLE Services(
ID integer not null,
SNum  integer not null auto_increment,
Branch varchar(250) not null,
SRank varchar(250),
UnitShip varchar(250),
primary key (SNum),
foreign key (ID) references Veterans (ID) ON UPDATE CASCADE ON DELETE CASCADE,
foreign key (Branch, SRank) references RanksList (BranchName, RankName) ON UPDATE CASCADE
);

CREATE TABLE Awards(
ID integer not null,
ANum  integer not null auto_increment,
AwardName varchar(250) not null,
BranchName varchar(250) not null,
primary key (ANum),
foreign key (ID) references Veterans (ID) ON UPDATE CASCADE ON DELETE CASCADE,
foreign key (BranchName, AwardName) references AwardsList (BranchName, AwardName) ON UPDATE CASCADE
);

CREATE TABLE Conflicts(
ID integer not null,
CNum  integer not null auto_increment,
ConflictName varchar(250) not null,
primary key (CNum),
foreign key (ID) references Veterans (ID) ON UPDATE CASCADE ON DELETE CASCADE,
foreign key (ConflictName) references ConflictList (CLName) ON UPDATE CASCADE
);

CREATE TABLE UserComments(
ID integer not null,
CNum  integer not null auto_increment,
UserComment varchar(3000) not null,
primary key (CNum),
foreign key (ID) references Veterans (ID) ON UPDATE CASCADE
);

