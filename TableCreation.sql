CREATE TABLE "Boards" (
	"Email"	TEXT NOT NULL,
	"IDgenerator"	INTEGER NOT NULL,
	PRIMARY KEY("Email")
);

CREATE TABLE "Columns" (
	"Email"	TEXT NOT NULL,
	"ColumnID"	INTEGER NOT NULL,
	"ColumnName"	Text NOT NULL,
	"ColumnLimit"	INTEGER NOT NULL,
	PRIMARY KEY("Email","ColumnID")
);

CREATE TABLE "Tasks" (
	"Email"	TEXT NOT NULL,
	"ColumnID" INTEGER NOT NULL,
	"TaskID" INTEGER NOT NULL,
	"Title" TEXT NOT NULL,
	"Description" TEXT,
	"CreationTime" TEXT NOT NULL,
	"DueDate" TEXT NOT NULL,
	PRIMARY KEY("Email","ColumnID","TaskID")
);

CREATE TABLE "Users" (
	"Email"	TEXT NOT NULL,
	"Password"	TEXT NOT NULL,
	"Nickname"	TEXT NOT NULL,
	PRIMARY KEY("Email")
);