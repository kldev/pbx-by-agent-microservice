export interface CalendarDay {
	date: Date;
	dayOfMonth: number;
	dayOfWeek: number; // 0 = Sunday, 6 = Saturday
	isWeekend: boolean;
	isCurrentMonth: boolean;
	dateString: string; // YYYY-MM-DD format
}

const POLISH_DAY_NAMES = ["Nie", "Pon", "Wto", "Śro", "Czw", "Pią", "Sob"];

const POLISH_DAY_NAMES_FULL = [
	"Niedziela",
	"Poniedziałek",
	"Wtorek",
	"Środa",
	"Czwartek",
	"Piątek",
	"Sobota",
];

/**
 * Get Polish day name (short)
 */
export function getPolishDayName(dayOfWeek: number): string {
	return POLISH_DAY_NAMES[dayOfWeek];
}

/**
 * Get Polish day name (full)
 */
export function getPolishDayNameFull(dayOfWeek: number): string {
	return POLISH_DAY_NAMES_FULL[dayOfWeek];
}

/**
 * Format date to YYYY-MM-DD
 */
export function formatDateString(date: Date): string {
	const year = date.getFullYear();
	const month = (date.getMonth() + 1).toString().padStart(2, "0");
	const day = date.getDate().toString().padStart(2, "0");
	return `${year}-${month}-${day}`;
}

/**
 * Get week number in year (ISO 8601)
 */
export function getWeekNumber(date: Date): number {
	const d = new Date(
		Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()),
	);
	const dayNum = d.getUTCDay() || 7;
	d.setUTCDate(d.getUTCDate() + 4 - dayNum);
	const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
	return Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
}

/**
 * Generate all days of a month
 */
export function generateMonthDays(year: number, month: number): CalendarDay[] {
	const days: CalendarDay[] = [];
	const daysInMonth = new Date(year, month, 0).getDate();

	for (let day = 1; day <= daysInMonth; day++) {
		const date = new Date(year, month - 1, day);
		const dayOfWeek = date.getDay();

		days.push({
			date,
			dayOfMonth: day,
			dayOfWeek,
			isWeekend: dayOfWeek === 0 || dayOfWeek === 6,
			isCurrentMonth: true,
			dateString: formatDateString(date),
		});
	}

	return days;
}

/**
 * Generate calendar grid (includes padding days from prev/next months)
 * First day of week is Monday (1)
 */
export function generateCalendarGrid(
	year: number,
	month: number,
): CalendarDay[] {
	const days: CalendarDay[] = [];
	const firstDayOfMonth = new Date(year, month - 1, 1);
	const lastDayOfMonth = new Date(year, month, 0);

	// Adjust first day: Monday = 0, Sunday = 6
	let firstDayOfWeek = firstDayOfMonth.getDay() - 1;
	if (firstDayOfWeek < 0) firstDayOfWeek = 6;

	// Add padding days from previous month
	const prevMonthLastDay = new Date(year, month - 1, 0).getDate();
	for (let i = firstDayOfWeek - 1; i >= 0; i--) {
		const day = prevMonthLastDay - i;
		const date = new Date(year, month - 2, day);
		const dayOfWeek = date.getDay();
		days.push({
			date,
			dayOfMonth: day,
			dayOfWeek,
			isWeekend: dayOfWeek === 0 || dayOfWeek === 6,
			isCurrentMonth: false,
			dateString: formatDateString(date),
		});
	}

	// Add current month days
	for (let day = 1; day <= lastDayOfMonth.getDate(); day++) {
		const date = new Date(year, month - 1, day);
		const dayOfWeek = date.getDay();
		days.push({
			date,
			dayOfMonth: day,
			dayOfWeek,
			isWeekend: dayOfWeek === 0 || dayOfWeek === 6,
			isCurrentMonth: true,
			dateString: formatDateString(date),
		});
	}

	// Add padding days from next month to complete the grid (6 rows * 7 days = 42)
	const remainingDays = 42 - days.length;
	for (let day = 1; day <= remainingDays; day++) {
		const date = new Date(year, month, day);
		const dayOfWeek = date.getDay();
		days.push({
			date,
			dayOfMonth: day,
			dayOfWeek,
			isWeekend: dayOfWeek === 0 || dayOfWeek === 6,
			isCurrentMonth: false,
			dateString: formatDateString(date),
		});
	}

	return days;
}

/**
 * Group days by week number
 */
export function groupByWeek(
	days: CalendarDay[],
): { weekNumber: number; days: CalendarDay[] }[] {
	const weeks: Map<number, CalendarDay[]> = new Map();

	for (const day of days) {
		const weekNum = getWeekNumber(day.date);
		if (!weeks.has(weekNum)) {
			weeks.set(weekNum, []);
		}
		weeks.get(weekNum)?.push(day);
	}

	return Array.from(weeks.entries())
		.sort(([a], [b]) => a - b)
		.map(([weekNumber, days]) => ({ weekNumber, days }));
}
