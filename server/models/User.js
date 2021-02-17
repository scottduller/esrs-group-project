const mongoose = require('mongoose'); // Erase if already required

// Declare the Schema of the Mongo model
const userSchema = new mongoose.Schema(
	{
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
		email_verified: {
			type: Boolean,
			default: false,
		},
		password: {
			type: String,
		},
		favourites: [
			{
				type: mongoose.Schema.Types.ObjectId,
				ref: 'Level',
				required: true,
			},
		],
		referral_code: {
			type: String,
			default: () => {
				let hash = 0;
				for (let i = 0; i < this.email.length; i++) {
					hash = this.email.charCodeAt(i) + ((hash << 5) - hash);
				}
				let res = (hash & 0x00ffffff).toString(16).toUpperCase();
				return '00000'.substring(0, 6 - res.length) + res;
			},
		},
		referred_by: {
			type: String,
			default: null,
		},
		third_party_auth: [ProviderSchema],
	},
	{ timestamps: true, strict: false }
);

const ProviderSchema = new mongoose.Schema({
	provider_name: {
		type: String,
		default: null,
	},
	provider_id: {
		type: String,
		default: null,
	},
	proider_data: {
		type: {},
		default: null,
	},
});

//Export the model
module.exports = mongoose.model('User', userSchema);
