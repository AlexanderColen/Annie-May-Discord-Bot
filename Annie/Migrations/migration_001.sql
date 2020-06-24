--Delete schema beforehand if it exists.
DROP SCHEMA annie_may CASCADE;

--Create a new schema.
CREATE SCHEMA annie_may;

--Create the User table.
CREATE TABLE annie_may.user (
	Id			SERIAL,
	DiscordId 	numeric(20, 0),
	Name		text,
	AnilistId	numeric(20, 0),
	AnilistName	text
);