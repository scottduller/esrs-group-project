const passport = require('passport');
const express = require('express');
const router = express.Router();

router.post('/', (req, res, next) => {
	passport.authenticate('local', (err, user, info) => {
		if (err) {
			return res.status(400).json({ error: err });
		}
		if (!user) {
			return res.status(400).json({ errors: 'No user found' });
		}
		req.logIn(user, (err) => {
			if (err) {
				return res.status(400).json({ errors: err });
			}
			return res
				.status(200)
				.json({ success: `logged in ${user.id}` });
		});
	})(req, res, next);
});

module.exports = router;
