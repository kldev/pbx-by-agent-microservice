/**
 * Convert hours and minutes to total minutes
 * @example toTotalMinutes(8, 30) => 510
 */
export function toTotalMinutes(hours: number, minutes: number): number {
	return hours * 60 + minutes;
}

/**
 * Convert total minutes to "H:MM" format
 * @example toTimeFormat(510) => "8:30"
 */
export function toTimeFormat(totalMinutes: number): string {
	const hours = Math.floor(totalMinutes / 60);
	const minutes = totalMinutes % 60;
	return `${hours}:${minutes.toString().padStart(2, "0")}`;
}

/**
 * Convert total minutes to decimal hours (for Excel)
 * @example toDecimalHours(510) => 8.5
 */
export function toDecimalHours(totalMinutes: number): number {
	return totalMinutes / 60;
}

/**
 * Extract hours from total minutes
 * @example getHours(510) => 8
 */
export function getHours(totalMinutes: number): number {
	return Math.floor(totalMinutes / 60);
}

/**
 * Extract remaining minutes from total minutes
 * @example getMinutes(510) => 30
 */
export function getMinutes(totalMinutes: number): number {
	return totalMinutes % 60;
}

/**
 * Calculate end time from start time and duration
 * @example calculateEndTime("08:00", 8, 30) => "16:30"
 */
export function calculateEndTime(
	startTime: string,
	hours: number,
	minutes: number,
): string {
	const [startHours, startMinutes] = startTime.split(":").map(Number);
	const totalMinutes =
		startHours * 60 + startMinutes + toTotalMinutes(hours, minutes);
	const endHours = Math.floor(totalMinutes / 60) % 24;
	const endMinutes = totalMinutes % 60;
	return `${endHours.toString().padStart(2, "0")}:${endMinutes.toString().padStart(2, "0")}`;
}

/**
 * Generate minute options (0, 5, 10, ..., 55)
 */
export function getMinuteOptions(): { value: number; label: string }[] {
	return Array.from({ length: 12 }, (_, i) => {
		const value = i * 5;
		return { value, label: value.toString().padStart(2, "0") };
	});
}

/**
 * Generate hour options (0-24)
 */
export function getHourOptions(): { value: number; label: string }[] {
	return Array.from({ length: 25 }, (_, i) => ({
		value: i,
		label: i.toString(),
	}));
}

/**
 * Generate start time options (every 15 minutes from 00:00 to 23:45)
 */
export function getStartTimeOptions(): { value: string; label: string }[] {
	const options: { value: string; label: string }[] = [];
	for (let h = 0; h < 24; h++) {
		for (let m = 0; m < 60; m += 15) {
			const value = `${h.toString().padStart(2, "0")}:${m.toString().padStart(2, "0")}`;
			options.push({ value, label: value });
		}
	}
	return options;
}

// ============================================================================
// FORMATOWANIE DLA API (zgodność frontend ↔ backend)
// Backend .NET używa wewnętrznie DateOnly i TimeOnly, ale przyjmuje stringi.
// Ustalony format:
//   - Data: ISO string "YYYY-MM-DD" (np. "2025-01-15")
//   - Czas: string "HH:mm" (np. "08:00")
// ============================================================================

/**
 * Formatuje Date do formatu API: "YYYY-MM-DD"
 * @example formatDateForApi(new Date(2025, 0, 15)) => "2025-01-15"
 */
export function formatDateForApi(date: Date): string {
	const year = date.getFullYear();
	const month = (date.getMonth() + 1).toString().padStart(2, "0");
	const day = date.getDate().toString().padStart(2, "0");
	return `${year}-${month}-${day}`;
}

/**
 * Formatuje godziny i minuty do formatu API: "HH:mm"
 * @example formatTimeForApi(8, 30) => "08:30"
 */
export function formatTimeForApi(hours: number, minutes: number): string {
	return `${hours.toString().padStart(2, "0")}:${minutes.toString().padStart(2, "0")}`;
}

/**
 * Parsuje string czasu "HH:mm" na obiekt { hours, minutes }
 * @example parseTimeString("08:30") => { hours: 8, minutes: 30 }
 */
export function parseTimeString(time: string): {
	hours: number;
	minutes: number;
} {
	const [hours, minutes] = time.split(":").map(Number);
	return { hours: hours || 0, minutes: minutes || 0 };
}

/**
 * Parsuje string daty "YYYY-MM-DD" na obiekt Date
 * @example parseDateString("2025-01-15") => Date(2025, 0, 15)
 */
export function parseDateString(date: string): Date {
	return new Date(date);
}
