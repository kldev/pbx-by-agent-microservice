import {
	Button,
	makeStyles,
	SearchBox,
	type SearchBoxChangeEvent,
	Text,
	tokens,
} from "@fluentui/react-components";
import {
	ChevronLeftRegular,
	ChevronRightRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router";

import {
	EmptyState,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { useDebounce } from "../../../hooks/useDebounce";
import type {
	CallRecordListFilter,
	CallRecordResponse,
} from "../../../api/cdr/models";
import { useGetCallRecordsList } from "../../../api/cdr/endpoints/call-records/call-records";
import { CdrTable } from "../components";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		height: "100%",
	},
	toolbar: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
		padding: tokens.spacingHorizontalL,
		paddingTop: tokens.spacingVerticalM,
		paddingBottom: tokens.spacingVerticalM,
		flexWrap: "wrap",
	},
	searchBox: {
		minWidth: "280px",
	},
	content: {
		flex: 1,
		padding: tokens.spacingHorizontalL,
		paddingTop: 0,
		overflowX: "auto",
	},
	pagination: {
		display: "flex",
		alignItems: "center",
		justifyContent: "flex-end",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingVerticalM,
		borderTop: `1px solid ${tokens.colorNeutralStroke2}`,
	},
	paginationInfo: {
		marginRight: tokens.spacingHorizontalM,
	},
});

const DEFAULT_PAGE_SIZE = 20;
const SEARCH_DEBOUNCE_MS = 300;

const CdrListPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();

	const [searchValue, setSearchValue] = useState("");
	const [pageNumber, setPageNumber] = useState(1);
	const debouncedSearch = useDebounce(searchValue, SEARCH_DEBOUNCE_MS);

	const filter: CallRecordListFilter = {
		pageNumber: pageNumber,
		pageSize: DEFAULT_PAGE_SIZE,
		search: debouncedSearch || undefined,
	};

	const cdrListMutation = useGetCallRecordsList();

	useEffect(() => {
		cdrListMutation.mutate({ data: filter });
	}, [pageNumber, debouncedSearch]);

	const result = cdrListMutation.data;
	const items: CallRecordResponse[] =
		result?.status === 200 ? (result.data.items ?? []) : [];
	const totalCount =
		result?.status === 200 ? (result.data.totalCount ?? 0) : 0;
	const totalPages =
		result?.status === 200 ? (result.data.totalPages ?? 1) : 1;
	const currentPage = pageNumber;
	const hasNextPage =
		result?.status === 200 ? (result.data.hasNextPage ?? false) : false;
	const hasPreviousPage =
		result?.status === 200 ? (result.data.hasPreviousPage ?? false) : false;
	const hasActiveFilters = !!searchValue;

	const handleRowClick = (item: CallRecordResponse) => {
		if (item.gid) {
			navigate(`/cdr/${item.gid}`);
		}
	};

	const handlePreviousPage = () => {
		if (hasPreviousPage) setPageNumber((prev) => prev - 1);
	};

	const handleNextPage = () => {
		if (hasNextPage) setPageNumber((prev) => prev + 1);
	};

	const handleSearchChange = useCallback(
		(_ev: SearchBoxChangeEvent, data: { value: string }) => {
			setSearchValue(data.value);
			setPageNumber(1);
		},
		[],
	);

	const handleSearchClear = useCallback(() => {
		setSearchValue("");
		setPageNumber(1);
	}, []);

	return (
		<div className={styles.container}>
			<PageHeader
				title="Historia połączeń (CDR)"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Historia połączeń" },
				]}
			/>
			<div className={styles.toolbar}>
				<SearchBox
					className={styles.searchBox}
					placeholder="Szukaj po numerze telefonu..."
					value={searchValue}
					onChange={handleSearchChange}
					dismiss={searchValue ? { onClick: handleSearchClear } : undefined}
				/>
			</div>
			<div className={styles.content}>
				{cdrListMutation.isPending && (
					<LoadingSpinner label="Ładowanie historii połączeń..." />
				)}
				{cdrListMutation.isError && (
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać historii połączeń."
						onRetry={() => cdrListMutation.mutate({ data: filter })}
					/>
				)}
				{cdrListMutation.isSuccess && items.length === 0 && (
					<EmptyState
						title={hasActiveFilters ? "Brak wyników" : "Brak połączeń"}
						description={
							hasActiveFilters
								? "Nie znaleziono połączeń pasujących do filtrów."
								: "Nie znaleziono żadnych rekordów połączeń."
						}
					/>
				)}
				{cdrListMutation.isSuccess && items.length > 0 && (
					<>
						<CdrTable items={items} onRowClick={handleRowClick} />
						<div className={styles.pagination}>
							<Text className={styles.paginationInfo}>
								Strona {currentPage} z {totalPages} (łącznie {totalCount}{" "}
								rekordów)
							</Text>
							<Button
								appearance="subtle"
								icon={<ChevronLeftRegular />}
								disabled={!hasPreviousPage}
								onClick={handlePreviousPage}
							>
								Poprzednia
							</Button>
							<Button
								appearance="subtle"
								icon={<ChevronRightRegular />}
								iconPosition="after"
								disabled={!hasNextPage}
								onClick={handleNextPage}
							>
								Następna
							</Button>
						</div>
					</>
				)}
			</div>
		</div>
	);
};

export default CdrListPage;
