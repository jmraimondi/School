INSERT INTO Cemeteries VALUES ('Roselawn','HIWY 31 & FORREST ST','Athens','34 47 41 92 N    86 57 06 45 W','Roselawn.png','RoselawnDirections.jpg');
INSERT INTO Cemeteries (CName, CCity) VALUES ('Roselawn','Decatur');
INSERT INTO Cemeteries (CName, CCity) VALUES ('Athens City Cemetery','Athens');
INSERT INTO Cemeteries (CName, CCity) VALUES ('Thatch-Mann','Athens');
INSERT INTO Cemeteries (CName, CCity) VALUES ('Sardis Springs Baptist Church Cemetery','Andersonville');

INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('John','Smith','1968-02-20','2018-06-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Jim','Smith','1970-02-20','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Bob','Johnson','1913-08-17','1988-02-14');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Richard','Jones','1935-01-27','2004-11-05');
INSERT INTO Veterans (FName, MName, LName, Suffix, DOB, DOD, CSection, CRow, MarkerLocation, MilPicLoc, CasualPicLoc, MiscPicLoc, MarkerPicLoc, CName, CCity)
 VALUES ('Lots','Of','Data','Jr.','1929-05-28','1989-10-01','5','2','Head','Eisenhower.jpg','DEisenhower.jpg','beret.jpg','Seventeen.jpg','Roselawn','Athens');

SET @LotsId = (SELECT ID FROM Veterans WHERE LName = 'Data');

INSERT INTO Services (ID, Branch, SRank, UnitShip) VALUES (@LotsId,'US ARMY','Private','82nd Airborne');
INSERT INTO Services (ID, Branch, SRank, UnitShip) VALUES (@LotsId,'US AIR FORCE','Senior Airman','13th Bomb Squadron');
INSERT INTO Services (ID, Branch, SRank, UnitShip) VALUES (@LotsId,'US NAVY','Seaman','USS Constitution');
INSERT INTO Services (ID, Branch, SRank, UnitShip) VALUES (@LotsId,'US MARINE CORPS','Sergeant','2nd Marine Division');

SET @CurId = (SELECT ID FROM Veterans WHERE LName = 'Smith' AND FName = 'John');

INSERT INTO Services (ID, Branch, SRank, UnitShip) VALUES (@CurId,'US Navy','Seaman Recruit','USS George Washington');

SET @CurId = (SELECT ID FROM Veterans WHERE LName = 'Johnson');

INSERT INTO Services (ID, Branch) VALUES (@CurId,'US Navy');

SET @CurId = (SELECT ID FROM Veterans WHERE LName = 'Jones');

INSERT INTO Services (ID, Branch, SRank) VALUES (@CurId,'US Army','Private');

INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Abe','Smith','1970-02-1','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Alan','Smith','1970-02-2','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Adam','Smith','1970-02-3','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Andrew','Smith','1970-02-4','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Alex','Smith','1970-02-5','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Bill','Smith','1970-02-6','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Bob','Smith','1970-02-7','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Burt','Smith','1970-02-8','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Charlie','Smith','1970-02-9','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Dale','Smith','1970-02-10','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Dan','Smith','1970-02-11','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Eric','Smith','1970-02-12','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Frank','Smith','1970-02-13','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Fred','Smith','1970-02-14','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Greg','Smith','1970-02-15','2019-02-15');
INSERT INTO Veterans (FName, LName, DOB, DOD) VALUES ('Gary','Smith','1970-02-16','2019-02-15');

UPDATE Veterans SET Comments = 'Lorem ipsum dolor amet iceland irony tofu hoodie mixtape godard XOXO sartorial man bun mustache pitchfork enamel pin air plant. Squid DIY tacos, small batch brooklyn tilde disrupt lyft. Shoreditch adaptogen pickled, gastropub artisan single-origin coffee put a bird on it selfies gluten-free knausgaard migas swag. Actually bespoke XOXO yuccie shabby chic health goth ramps williamsburg hella. Brooklyn disrupt art party cornhole cray williamsburg kinfolk semiotics cliche sustainable. Ethical letterpress chicharrones kombucha hexagon kinfolk cray. Authentic venmo activated charcoal tbh polaroid jianbing cornhole. Mixtape microdosing 8-bit, jianbing 90s cred palo santo vaporware +1 XOXO occupy hammock post-ironic. Health goth shabby chic.' WHERE ID = @LotsId;

INSERT INTO Conflicts (ID, ConflictName) VALUES (@LotsId,'WW I');
INSERT INTO Conflicts (ID, ConflictName) VALUES (@LotsId,'WW II');

INSERT INTO Awards (ID, BranchName, AwardName) VALUES (@LotsId,'US ARMY','Purple Heart');
INSERT INTO Awards (ID, BranchName, AwardName) VALUES (@LotsId,'US ARMY','Medal of Honor');