import {
	Button,
	Dropdown,
	makeStyles,
	Option,
	SearchBox,
	type SearchBoxChangeEvent,
	Text,
	tokens,
} from "@fluentui/react-components";
import { ChevronLeftRegular, ChevronRightRegular } from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useEffect, useState } from "react";

import {
	EmptyState,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { useDebounce } from "../../../hooks/useDebounce";
import type {
	DocumentEntryListFilter,
	DocumentEntryResponse,
	DocumentTypeResponse,
	CurrencyTypeResponse,
} from "../../../api/fincosts/models";
import {
	useGetDocumentEntries,
	useGetDocumentTypes,
	useGetCurrencyTypes,
} from "../../../api/fincosts/endpoints/costs/costs";
import { CostsTable } from "../components";

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
	dropdown: {
		minWidth: "160px",
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

const PAYMENT_STATUS_ALL = "__all__";
const PAYMENT_STATUS_PAID = "__paid__";
const PAYMENT_STATUS_UNPAID = "__unpaid__";

const CostsListPage: React.FC = () => {
	const styles = useStyles();

	const [searchValue, setSearchValue] = useState("");
	const [pageNumber, setPageNumber] = useState(1);
	const [selectedDocTypeId, setSelectedDocTypeId] = useState<number | null>(null);
	const [selectedCurrency, setSelectedCurrency] = useState<string | null>(null);
	const [selectedPaymentStatus, setSelectedPaymentStatus] = useState<string>(PAYMENT_STATUS_ALL);
	const debouncedSearch = useDebounce(searchValue, SEARCH_DEBOUNCE_MS);

	// Dictionary data
	const docTypesMutation = useGetDocumentTypes();
	const currencyTypesMutation = useGetCurrencyTypes();

	useEffect(() => {
		docTypesMutation.mutate();
		currencyTypesMutation.mutate();
	}, []);

	const docTypes: DocumentTypeResponse[] =
		docTypesMutation.data?.status === 200 ? docTypesMutation.data.data : [];
	const currencyTypes: CurrencyTypeResponse[] =
		currencyTypesMutation.data?.status === 200 ? currencyTypesMutation.data.data : [];

	// Build filter
	const wasPaidFilter =
		selectedPaymentStatus === PAYMENT_STATUS_PAID
			? true
			: selectedPaymentStatus === PAYMENT_STATUS_UNPAID
				? false
				: undefined;

	const filter: DocumentEntryListFilter = {
		pageNumber: pageNumber,
		pageSize: DEFAULT_PAGE_SIZE,
		search: debouncedSearch || undefined,
		documentTypeId: selectedDocTypeId ?? undefined,
		currencyNamePL: selectedCurrency ?? undefined,
		wasPaid: wasPaidFilter,
	};

	const documentsMutation = useGetDocumentEntries();

	useEffect(() => {
		documentsMutation.mutate({ data: filter });
	}, [pageNumber, debouncedSearch, selectedDocTypeId, selectedCurrency, selectedPaymentStatus]);

	const result = documentsMutation.data;
	const items: DocumentEntryResponse[] =
		result?.status === 200 ? (result.data.items ?? []) : [];
	const totalCount = result?.status === 200 ? (result.data.totalCount ?? 0) : 0;
	const totalPages = result?.status === 200 ? (result.data.totalPages ?? 1) : 1;
	const currentPage = pageNumber;
	const hasNextPage =
		result?.status === 200 ? (result.data.hasNextPage ?? false) : false;
	const hasPreviousPage =
		result?.status === 200 ? (result.data.hasPreviousPage ?? false) : false;
	const hasActiveFilters =
		!!searchValue || selectedDocTypeId !== null || selectedCurrency !== null || selectedPaymentStatus !== PAYMENT_STATUS_ALL;

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
				title="Koszty"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Koszty" },
				]}
			/>
			<div className={styles.toolbar}>
				<SearchBox
					className={styles.searchBox}
					placeholder="Szukaj po wystawcy, odbiorcy, nr KSeF..."
					value={searchValue}
					onChange={handleSearchChange}
					dismiss={searchValue ? { onClick: handleSearchClear } : undefined}
				/>
				<Dropdown
					className={styles.dropdown}
					placeholder="Typ dokumentu"
					value={
						selectedDocTypeId !== null
							? (docTypes.find((d) => d.id === selectedDocTypeId)?.namePL ?? "")
							: ""
					}
					selectedOptions={selectedDocTypeId !== null ? [String(selectedDocTypeId)] : []}
					onOptionSelect={(_ev, data) => {
						const val = data.optionValue;
						setSelectedDocTypeId(val === "__all__" ? null : Number(val));
						setPageNumber(1);
					}}
				>
					<Option value="__all__">Wszystkie typy</Option>
					{docTypes.map((dt) => (
						<Option key={dt.id} value={String(dt.id)}>
							{dt.namePL || ""}
						</Option>
					))}
				</Dropdown>
				<Dropdown
					className={styles.dropdown}
					placeholder="Waluta"
					value={selectedCurrency ?? ""}
					selectedOptions={selectedCurrency !== null ? [selectedCurrency] : []}
					onOptionSelect={(_ev, data) => {
						const val = data.optionValue;
						setSelectedCurrency(val === "__all__" ? null : (val ?? null));
						setPageNumber(1);
					}}
				>
					<Option value="__all__">Wszystkie waluty</Option>
					{currencyTypes.map((ct) => (
						<Option key={ct.id} value={ct.namePL || ""}>
							{ct.namePL || ""}
						</Option>
					))}
				</Dropdown>
				<Dropdown
					className={styles.dropdown}
					placeholder="Status płatności"
					value={
						selectedPaymentStatus === PAYMENT_STATUS_PAID
							? "Opłacone"
							: selectedPaymentStatus === PAYMENT_STATUS_UNPAID
								? "Nieopłacone"
								: ""
					}
					selectedOptions={selectedPaymentStatus !== PAYMENT_STATUS_ALL ? [selectedPaymentStatus] : []}
					onOptionSelect={(_ev, data) => {
						setSelectedPaymentStatus(data.optionValue ?? PAYMENT_STATUS_ALL);
						setPageNumber(1);
					}}
				>
					<Option value={PAYMENT_STATUS_ALL}>Wszystkie</Option>
					<Option value={PAYMENT_STATUS_PAID}>Opłacone</Option>
					<Option value={PAYMENT_STATUS_UNPAID}>Nieopłacone</Option>
				</Dropdown>
			</div>
			<div className={styles.content}>
				{documentsMutation.isPending && (
					<LoadingSpinner label="Ładowanie dokumentów kosztowych..." />
				)}
				{documentsMutation.isError && (
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać dokumentów kosztowych."
						onRetry={() => documentsMutation.mutate({ data: filter })}
					/>
				)}
				{documentsMutation.isSuccess && items.length === 0 && (
					<EmptyState
						title={hasActiveFilters ? "Brak wyników" : "Brak dokumentów"}
						description={
							hasActiveFilters
								? "Nie znaleziono dokumentów pasujących do filtrów."
								: "Nie znaleziono żadnych dokumentów kosztowych."
						}
					/>
				)}
				{documentsMutation.isSuccess && items.length > 0 && (
					<>
						<CostsTable items={items} />
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

export default CostsListPage;
