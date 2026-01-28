import { makeStyles, Text, tokens } from "@fluentui/react-components";
import type React from "react";
import type { DayEntryResponse } from "../../../../api/rcp/models";
import { generateCalendarGrid } from "../../utils";
import DayCell from "./DayCell";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalS,
	},
	header: {
		display: "grid",
		gridTemplateColumns: "repeat(7, 1fr)",
		gap: tokens.spacingHorizontalXS,
	},
	headerCell: {
		textAlign: "center",
		padding: tokens.spacingVerticalS,
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	grid: {
		display: "grid",
		gridTemplateColumns: "repeat(7, 1fr)",
		gap: tokens.spacingHorizontalXS,
	},
});

interface MonthCalendarProps {
	year: number;
	month: number;
	days: DayEntryResponse[];
	canEdit: boolean;
	onDayClick: (dateString: string) => void;
}

const WEEKDAY_HEADERS = ["Pon", "Wto", "Śro", "Czw", "Pią", "Sob", "Nie"];

const MonthCalendar: React.FC<MonthCalendarProps> = ({
	year,
	month,
	days,
	canEdit,
	onDayClick,
}) => {
	const styles = useStyles();
	const calendarDays = generateCalendarGrid(year, month);

	const dayEntriesMap = new Map(days.map((d) => [d.workDate, d]));

	return (
		<div className={styles.container}>
			<div className={styles.header}>
				{WEEKDAY_HEADERS.map((day) => (
					<Text key={day} className={styles.headerCell}>
						{day}
					</Text>
				))}
			</div>
			<div className={styles.grid}>
				{calendarDays.map((calDay) => (
					<DayCell
						key={calDay.dateString}
						calendarDay={calDay}
						entry={dayEntriesMap.get(calDay.dateString)}
						canEdit={canEdit && calDay.isCurrentMonth}
						onClick={() =>
							calDay.isCurrentMonth && onDayClick(calDay.dateString)
						}
					/>
				))}
			</div>
		</div>
	);
};

export default MonthCalendar;
