import { makeStyles, Text, tokens } from "@fluentui/react-components";
import type React from "react";
import type { DayEntryResponse } from "../../../../api/rcp/models";
import { generateMonthDays, groupByWeek } from "../../utils";
import DayCard from "./DayCard";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalS,
	},
	weekHeader: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		padding: `${tokens.spacingVerticalS} 0`,
	},
	weekLine: {
		flex: 1,
		height: "1px",
		backgroundColor: tokens.colorNeutralStroke2,
	},
	weekLabel: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
		fontWeight: tokens.fontWeightSemibold,
	},
	weekDays: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
});

interface DaysListProps {
	year: number;
	month: number;
	days: DayEntryResponse[];
	canEdit: boolean;
	onDayClick: (dateString: string) => void;
}

const DaysList: React.FC<DaysListProps> = ({
	year,
	month,
	days,
	canEdit,
	onDayClick,
}) => {
	const styles = useStyles();
	const monthDays = generateMonthDays(year, month);
	const weekGroups = groupByWeek(monthDays);

	const dayEntriesMap = new Map(days.map((d) => [d.workDate, d]));

	return (
		<div className={styles.container}>
			{weekGroups.map(({ weekNumber, days: weekDays }) => (
				<div key={weekNumber}>
					<div className={styles.weekHeader}>
						<div className={styles.weekLine} />
						<Text className={styles.weekLabel}>Tydzień {weekNumber}</Text>
						<div className={styles.weekLine} />
					</div>
					<div className={styles.weekDays}>
						{weekDays.map((calDay) => (
							<DayCard
								key={calDay.dateString}
								calendarDay={calDay}
								entry={dayEntriesMap.get(calDay.dateString)}
								canEdit={canEdit}
								onClick={() => onDayClick(calDay.dateString)}
							/>
						))}
					</div>
				</div>
			))}
		</div>
	);
};

export default DaysList;
