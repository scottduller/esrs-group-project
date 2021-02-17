const bcrypt = require('bcryptjs');
const User = require('../models/User');
const passport = require('passport');
const LocalStrategy = require('passport-local').Strategy;

passport.serializeUser((user, done) => {
	done(null, user.id);
});

passport.deserializeUser((id, done) => {
	User.findById(id, (err, user) => {
		done(err, user);
	});
});

passport.use(
	new LocalStrategy(
		{ usernameField: 'email' },
		async (email, password, done) => {
			try {
				const user = await User.findOne({ email: email });

				if (!user) {
					const newUser = new User({ email, password });

					bcrypt.genSalt(10, (err, salt) => {
						bcrypt.hash(newUser.password, salt, async (err, hash) => {
							if (err) throw err;
							newUser.password = hash;
							user = newUser.save();

							return done(null, user);
						});
					});
				} else {
					bcrypt.compare(password, user.password, (err, isMatch) => {
						if (err) throw err;

						if (isMatch) {
							return done(null, user);
						} else {
							return done(null, false, { message: 'Wrong password' });
						}
					});
				}
			} catch (err) {
				return done(null, false, { message: err });
			}
		}
	)
);

module.exports = passport;
