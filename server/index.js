const express = require('express');
const cookieSession = require('cookie-session');

const morgan = require('morgan');

const passport = require('./utils/passport');
const connectDB = require('./config/db');
const keys = require('./config/keys');

require('./models/Level');
require('./models/User');
require('./models/Playlist');

const app = express();

connectDB();

app.use(express.json());
app.use(express.urlencoded({ extended: false }));

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

app.use('/api/auth', require('./routes/authRoutes'));
app.use('/api/levels', require('./routes/levelRoutes'));
app.use('/api/playlists', require('./routes/playlistRoutes'));

const PORT = process.env.PORT || 5000;

app.listen(PORT, () => {
	console.log(`SUCCESS ... Listening on port ${PORT}`);
});
