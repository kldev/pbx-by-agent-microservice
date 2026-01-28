import {
	Button,
	MessageBar,
	MessageBarBody,
	makeStyles,
	Spinner,
	Text,
	tokens,
} from "@fluentui/react-components";
import { SendRegular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect, useState } from "react";
import {
	useGetMyMonthlyEntry,
	useSubmitEntry,
} from "../../../../api/rcp/endpoints/rcp-employee/rcp-employee";
import { RcpTimeEntryStatus } from "../../../../api/rcp/models";
import { useDeviceType } from "../../hooks";
import { toTimeFormat } from "../../utils";
import RcpStatusBadge from "../RcpStatusBadge";
import {
	HintHighlight,
	HintList,
	HintListItem,
	HintSection,
	HintWarning,
	SectionHint,
} from "../shared";
import DaysList from "./DaysList";
import MonthCalendar from "./MonthCalendar";
import TimeEntryDialog from "./TimeEntryDialog";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalL,
	},
	summary: {
		display: "flex",
		alignItems: "center",
		justifyContent: "space-between",
		padding: tokens.spacingHorizontalL,
		backgroundColor: tokens.colorNeutralBackground2,
		borderRadius: tokens.borderRadiusMedium,
		flexWrap: "wrap",
		gap: tokens.spacingHorizontalM,
	},
	summaryLeft: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalL,
	},
	summaryItem: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	label: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	value: {
		fontSize: tokens.fontSizeBase500,
		fontWeight: tokens.fontWeightSemibold,
	},
	loading: {
		display: "flex",
		justifyContent: "center",
		alignItems: "center",
		padding: tokens.spacingVerticalXXL,
	},
});

interface MyMonthViewProps {
	year: number;
	month: number;
}

const MyMonthView: React.FC<MyMonthViewProps> = ({ year, month }) => {
	const styles = useStyles();
	const { isMobile } = useDeviceType();
	const [selectedDate, setSelectedDate] = useState<string | null>(null);

	const getEntryMutation = useGetMyMonthlyEntry();
	const submitMutation = useSubmitEntry();

	const { data, isPending: isLoading, isError } = getEntryMutation;

	// Fetch data when year/month changes
	useEffect(() => {
		getEntryMutation.mutate({ data: { year, month } });
	}, [year, month]);

	const refetch = () => {
		getEntryMutation.mutate({ data: { year, month } });
	};

	const entry = data?.status === 200 ? data.data : null;
	const days = entry?.days ?? [];
	const totalMinutes = entry?.totalMinutes ?? 0;
	const status = entry?.status ?? RcpTimeEntryStatus.Draft;

	const canEdit =
		status === RcpTimeEntryStatus.Draft ||
		status === RcpTimeEntryStatus.Correction;
	const canSubmit =
		status === RcpTimeEntryStatus.Draft ||
		status === RcpTimeEntryStatus.Correction;

	const handleDayClick = (dateString: string) => {
		if (canEdit) {
			setSelectedDate(dateString);
		}
	};

	const handleDialogClose = () => {
		setSelectedDate(null);
	};

	const handleDialogSuccess = () => {
		setSelectedDate(null);
		refetch();
	};

	const handleSubmit = async () => {
		try {
			await submitMutation.mutateAsync({
				data: { year, month, comment: null },
			});
			refetch();
		} catch (error) {
			console.error("Failed to submit entry:", error);
		}
	};

	if (isLoading) {
		return (
			<div className={styles.loading}>
				<Spinner label="Ładowanie..." />
			</div>
		);
	}

	if (isError) {
		return (
			<MessageBar intent="error">
				<MessageBarBody>
					Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.
				</MessageBarBody>
			</MessageBar>
		);
	}

	return (
		<div className={styles.container}>
			<SectionHint
				title="Jak wpisywać godziny pracy?"
				storageKey="rcp_my_month"
			>
				<HintSection title="Wpisywanie godzin">
					Kliknij w dzień w kalendarzu, aby wpisać swoje godziny pracy. Podaj
					godzinę rozpoczęcia i czas pracy - system obliczy godzinę zakończenia.
				</HintSection>

				<HintSection title="Statusy miesiąca">
					<HintList>
						<HintListItem>
							<HintHighlight>Wpisywanie</HintHighlight> - możesz edytować wpisy
						</HintListItem>
						<HintListItem>
							<HintHighlight>Wysłane</HintHighlight> - czeka na zatwierdzenie
							przez przełożonego
						</HintListItem>
						<HintListItem>
							<HintHighlight>Zatwierdzone</HintHighlight> - zaakceptowane, nie
							można edytować
						</HintListItem>
						<HintListItem>
							<HintHighlight>Do poprawy</HintHighlight> - przełożony zwrócił do
							korekty
						</HintListItem>
						<HintListItem>
							<HintHighlight>Rozliczenie</HintHighlight> - przekazane do kadr
						</HintListItem>
					</HintList>
				</HintSection>

				<HintSection>
					<HintWarning>Ważne:</HintWarning> Po kliknięciu "Wyślij do
					zatwierdzenia" nie będziesz mógł edytować wpisów - chyba że przełożony
					zwróci miesiąc do poprawy. Wpisuj godziny regularnie, najlepiej
					codziennie.
				</HintSection>
			</SectionHint>

			<div className={styles.summary}>
				<div className={styles.summaryLeft}>
					<div className={styles.summaryItem}>
						<Text className={styles.label}>Suma godzin</Text>
						<Text className={styles.value}>{toTimeFormat(totalMinutes)}</Text>
					</div>
					<div className={styles.summaryItem}>
						<Text className={styles.label}>Status</Text>
						<RcpStatusBadge status={status} />
					</div>
				</div>
				{canSubmit && days.length > 0 && (
					<Button
						appearance="primary"
						icon={<SendRegular />}
						onClick={handleSubmit}
						disabled={submitMutation.isPending}
					>
						{submitMutation.isPending
							? "Wysyłanie..."
							: "Wyślij do zatwierdzenia"}
					</Button>
				)}
			</div>

			{isMobile ? (
				<DaysList
					year={year}
					month={month}
					days={days}
					canEdit={canEdit}
					onDayClick={handleDayClick}
				/>
			) : (
				<MonthCalendar
					year={year}
					month={month}
					days={days}
					canEdit={canEdit}
					onDayClick={handleDayClick}
				/>
			)}

			{selectedDate && (
				<TimeEntryDialog
					year={year}
					month={month}
					date={selectedDate}
					existingEntry={days.find((d) => d.workDate === selectedDate)}
					onClose={handleDialogClose}
					onSuccess={handleDialogSuccess}
				/>
			)}
		</div>
	);
};

export default MyMonthView;
