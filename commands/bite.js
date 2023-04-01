/* eslint-disable indent */
const { SlashCommandBuilder } = require('discord.js');
const { userMention } = require('@discordjs/builders');
const { simulateEvent } = require('../helpers/probability.js');

module.exports = {
	data: new SlashCommandBuilder()
		.setName('bite')
		.setDescription('Bites a user')
		.addUserOption(option =>
			option
				.setName('target')
				.setDescription('Who to bite')
				.setRequired(true)),
	async execute(interaction) {
		const target = interaction.options.getMember('target');
		console.log(target.id);
		const event = simulateEvent([9, 1]);
		switch (event) {
			case 1:
				interaction.reply(`Biting ${userMention(target.id)}\n${userMention(target.id)} flinched`);
				break;
			default:
				interaction.reply('Biting ' + userMention(target.id));
		}
	},
};