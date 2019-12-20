CREATE USER 'viewerapp'@'%' IDENTIFIED BY 'InsertPasswordHere';
CREATE USER 'adminapp'@'%' IDENTIFIED BY 'InsertPasswordHere';

GRANT SELECT ON VeteransMuseum.* TO 'viewerapp'@'%';
GRANT INSERT ON VeteransMuseum.UserComments TO 'viewerapp'@'%';

GRANT ALL PRIVILEGES ON VeteransMuseum.* TO 'adminapp'@'%';