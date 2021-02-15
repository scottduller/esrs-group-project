const mongoose = require('mongoose'); // Erase if already required

// Declare the Schema of the Mongo model
var levelSchema = new mongoose.Schema(
	{
		user: {
			type: mongoose.Schema.Types.ObjectId,
			ref: 'User',
		},
		name: {
			type: String,
			required: true,
			unique: true,
		},
		description: {
			type: String,
			default: '',
		},
		votes: {
			type: Number,
			default: 0,
		},
		favorites: {
			type: Number,
			default: 0,
		},
		levelData: {
			type: String,
			required: true,
		},
	},
	{ timestamps: true }
);

//Export the model
module.exports = mongoose.model('Level', levelSchema);
