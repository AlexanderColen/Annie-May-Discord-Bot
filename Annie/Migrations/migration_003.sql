-- Delete status table if it exists.
DROP TABLE IF EXISTS annie_may.guild_settings;

-- Add status table.
CREATE TABLE annie_may.guild_settings (
	Id				SERIAL,
	GuildId 		numeric(20, 0),
	Prefix			text,
	ShowUserScores	boolean
);