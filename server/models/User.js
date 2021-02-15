const mongoose = require('mongoose'); // Erase if already required

// Declare the Schema of the Mongo model
var userSchema = new mongoose.Schema({
	name: {
		type: String,
		required: true,
		unique: true,
	},
	email: {
		type: String,
		required: true,
		unique: true,
	},
	password: {
		type: String,
		required: true,
	},
	favourites: [
		{
			type: mongoose.Schema.Types.ObjectId,
			ref: 'Level',
			required: true,
		},
	],
});

//Export the model
module.exports = mongoose.model('User', userSchema);
