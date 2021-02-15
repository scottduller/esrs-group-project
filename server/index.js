const express = require('express');
const morgan = require('morgan');
const cookieSession = require('cookie-session');
const passport = require('passport');
const connectDB = require('./config/db');
const keys = require('./config/keys');

const app = express();

require('./models/Level');
require('./models/User');
require('./models/Playlist');

connectDB();

app.use(express.json());

if (process.env.NODE_ENV === 'development') {
	app.use(morgan('dev'));
}
if (process.env.NODE_ENV === 'production') {
	app.use(
		morgan('tiny', {
			skip: function (req, res) {
				return res.statusCode < 400;
			},
		})
	);
}

app.use(
	cookieSession({
		maxAge: 30 * 24 * 60 * 60 * 1000,
		keys: [keys.cookieKey],
	})
);

app.use(passport.initialize());
app.use(passport.session());

app.use('/api/levels', require('./routes/levelRoutes'));
app.use('/api/playlists', require('./routes/playlistRoutes'));
app.use('/api/auth', require('./routes/authRoutes'));

const PORT = process.env.PORT || 5000;

app.listen(PORT, () => {
	console.log(`SUCCESS ... Listening on port ${PORT}`);
});

module.exports = app;
