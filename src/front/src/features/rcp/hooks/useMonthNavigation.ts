import { useCallback, useState } from "react";

interface UseMonthNavigationResult {
	year: number;
	month: number;
	goToPrevious: () => void;
	goToNext: () => void;
	goToToday: () => void;
	setMonth: (year: number, month: number) => void;
}

export function useMonthNavigation(): UseMonthNavigationResult {
	const now = new Date();
	const [year, setYear] = useState(now.getFullYear());
	const [month, setMonthState] = useState(now.getMonth() + 1);

	const goToPrevious = useCallback(() => {
		setMonthState((prev) => {
			if (prev === 1) {
				setYear((y) => y - 1);
				return 12;
			}
			return prev - 1;
		});
	}, []);

	const goToNext = useCallback(() => {
		const now = new Date();
		const currentYear = now.getFullYear();
		const currentMonth = now.getMonth() + 1;

		setMonthState((prev) => {
			const nextMonth = prev === 12 ? 1 : prev + 1;
			const nextYear = prev === 12 ? year + 1 : year;

			// Block navigation to future months
			if (
				nextYear > currentYear ||
				(nextYear === currentYear && nextMonth > currentMonth)
			) {
				return prev;
			}

			if (prev === 12) {
				setYear((y) => y + 1);
				return 1;
			}
			return prev + 1;
		});
	}, [year]);

	const goToToday = useCallback(() => {
		const now = new Date();
		setYear(now.getFullYear());
		setMonthState(now.getMonth() + 1);
	}, []);

	const setMonth = useCallback((newYear: number, newMonth: number) => {
		setYear(newYear);
		setMonthState(newMonth);
	}, []);

	return {
		year,
		month,
		goToPrevious,
		goToNext,
		goToToday,
		setMonth,
	};
}
