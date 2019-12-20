echo Merging SQL Files ...
copy /b "Veterans Schema.sql" MasterWithoutData.sql
copy /b MasterWithoutData.sql+"Branches.sql" MasterWithoutData.sql
copy /b MasterWithoutData.sql+"Conflicts.sql" MasterWithoutData.sql
copy /b MasterWithoutData.sql+"Ranks.sql" MasterWithoutData.sql
copy /b MasterWithoutData.sql+"Awards.sql" MasterWithoutData.sql
copy /b MasterWithoutData.sql+"Cemeteries.sql" MasterWithoutData.sql
copy /b MasterWithoutData.sql+"Fake Veteran Data.sql" MasterWithData.sql