import { Button, makeStyles, Text, tokens } from "@fluentui/react-components";
import {
	ArrowLeftRegular,
	ArrowRightRegular,
	CalendarTodayRegular,
} from "@fluentui/react-icons";
import type React from "react";

const useStyles = makeStyles({
	container: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
	},
	monthLabel: {
		minWidth: "160px",
		textAlign: "center",
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase400,
	},
});

interface MonthNavigatorProps {
	year: number;
	month: number;
	onPrevious: () => void;
	onNext: () => void;
	onToday: () => void;
}

const MONTH_NAMES = [
	"Styczeń",
	"Luty",
	"Marzec",
	"Kwiecień",
	"Maj",
	"Czerwiec",
	"Lipiec",
	"Sierpień",
	"Wrzesień",
	"Październik",
	"Listopad",
	"Grudzień",
];

const MonthNavigator: React.FC<MonthNavigatorProps> = ({
	year,
	month,
	onPrevious,
	onNext,
	onToday,
}) => {
	const styles = useStyles();

	const now = new Date();
	const currentYear = now.getFullYear();
	const currentMonth = now.getMonth() + 1;

	const isCurrentMonth = year === currentYear && month === currentMonth;
	const isFutureMonth =
		year > currentYear || (year === currentYear && month > currentMonth);

	return (
		<div className={styles.container}>
			<Button
				appearance="subtle"
				icon={<ArrowLeftRegular />}
				onClick={onPrevious}
				title="Poprzedni miesiąc"
			/>
			<Text className={styles.monthLabel}>
				{MONTH_NAMES[month - 1]} {year}
			</Text>
			<Button
				appearance="subtle"
				icon={<ArrowRightRegular />}
				onClick={onNext}
				disabled={isFutureMonth || isCurrentMonth}
				title="Następny miesiąc"
			/>
			<Button
				appearance="subtle"
				icon={<CalendarTodayRegular />}
				onClick={onToday}
				disabled={isCurrentMonth}
				title="Bieżący miesiąc"
			>
				Dziś
			</Button>
		</div>
	);
};

export default MonthNavigator;
