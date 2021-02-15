const mongoose = require('mongoose');
const requireLogin = require('../middleware/requireLogin');
const express = require('express');
const router = express.Router();

const Level = mongoose.model('Level');

router.get('/:id', requireLogin, async (req, res) => {
	const level = await Level.findOne({
		_id: req.params.id,
	});

	res.send(level);
});

router.get('/', requireLogin, async (req, res) => {
	const levels = await Level.find();

	res.send(levels);
});

router.get('/user', requireLogin, async (req, res) => {
	const levels = await Level.find({
		user: req.user.id,
	});

	res.send(levels);
});

router.post('/', requireLogin, async (req, res) => {
	const {
		name,
		description,
		votes,
		favourites,
		levelData,
	} = req.body;

	const level = new Level({
		name,
		description,
		votes,
		favourites,
		levelData,
		user: req.user.id,
	});

	try {
		await level.save();
		res.send(level);
	} catch (err) {
		res.send(400, err);
	}
});

router.put('/:id', requireLogin, async (req, res) => {
	const { name, levels } = req.body;

	try {
		const level = await Level.findOne({
			user: req.user.id,
			_id: req.params.id,
		});

		level.name = name;
		level.description = description;
		level.votes = votes;
		level.favourites = favourites;
		level.levelData = levelData;

		await level.save();
		res.send(level);
	} catch (err) {
		res.send(500, err);
	}
});

router.delete('/:id', requireLogin, async (req, res) => {
	try {
		const level = await Level.deleteOne({
			_id: req.params.id,
			user: req.user.id,
		});
	} catch (err) {
		res.send(500, err);
	}
});

module.exports = router;
