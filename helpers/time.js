function GetCountdown(month, day) {
	const today = new Date();
	const target = new Date(today.getFullYear(), (month - 1), day);
	if (today > target) target.setFullYear(today.getFullYear() + 1);
	return Math.ceil((target - today) / 86400000);
}

module.exports = { GetCountdown };