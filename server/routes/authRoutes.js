const passport = require('passport');
const express = require('express');
const router = express.Router();

router.get(
	'/google',
	passport.authenticate('google', {
		scope: ['profile', 'email'],
	})
);

router.get('/google/callback', passport.authenticate('google'));

router.get('/logout', (req, res) => {
	req.logout();
});

router.get('/current_user', (req, res) => {
	res.send(req.user);
});

module.exports = router;
