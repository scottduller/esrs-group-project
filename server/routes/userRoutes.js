const mongoose = require('mongoose');
const requireLogin = require('../middleware/requireLogin');
const express = require('express');
const router = express.Router();

const User = require('../models/User');

router.get('/', requireLogin, async (req, res) => {
	const user = await User.findOne({
		_id: req.user.id,
	});

	res.send(user);
});

router.put('/', requireLogin, async (req, res) => {
	const { name, favourites } = req.body;

	try {
		const user = await User.findOne({
			_id: req.user.id,
		});

		user.name = name;
		user.favourites = favourites;

		await user.save();
		res.send(user);
	} catch (err) {
		res.send(500, err);
	}
});

module.exports = router;
