function simulateEvent(chances) {
	let sum = 0;
	chances.forEach((chance) => {
		sum += chance;
	});
	const rand = Math.random();
	let chance = 0;
	for (let i = 0; i < chances.length; i++) {
		chance += chances[i] / sum;
		if (rand < chance) {
			return i;
		}
	}
	return -1;
}

module.exports = { simulateEvent };