const mongoose = require('mongoose');
const requireLogin = require('../middleware/requireLogin');
const express = require('express');
const router = express.Router();

const Playlist = mongoose.model('Playlist');

router.get('/:id', requireLogin, async (req, res) => {
	const playlist = await Playlist.findOne({
		user: req.user.id,
		_id: req.params.id,
	});

	res.send(playlist);
});

router.get('/', requireLogin, async (req, res) => {
	const playlists = await Playlist.find({
		user: req.user.id,
	});

	res.send(playlists);
});

router.post('/', requireLogin, async (req, res) => {
	const { name, levels } = req.body;

	const playlist = new Playlist({
		name,
		levels,
		user: req.user.id,
	});

	try {
		await playlist.save();
		res.send(playlist);
	} catch (err) {
		res.send(400, err);
	}
});

router.put('/:id', requireLogin, async (req, res) => {
	const { name, levels } = req.body;

	try {
		const playlist = await Playlist.findOne({
			user: req.user.id,
			_id: req.params.id,
		});

		playlist.name = name;
		playlist.levels = levels;

		await playlist.save();
		res.send(playlist);
	} catch (err) {
		res.send(500, err);
	}
});

router.delete('/:id', requireLogin, async (req, res) => {
	try {
		const playlist = await Playlist.deleteOne({
			_id: req.params.id,
			user: req.user.id,
		});
	} catch (err) {
		res.send(500, err);
	}
});

module.exports = router;
