-- Delete status table if it exists.
DROP TABLE IF EXISTS annie_may.status;

-- Add status table.
CREATE TABLE annie_may.status (
	Id			SERIAL,
	statusText 	text
);