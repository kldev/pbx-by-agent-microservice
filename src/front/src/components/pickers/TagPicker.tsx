import {
	Field,
	makeStyles,
	mergeClasses,
	Portal,
	Spinner,
	Tag,
	tokens,
} from "@fluentui/react-components";
import { ChevronDownRegular } from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useDebounce } from "../../hooks/useDebounce";

const useStyles = makeStyles({
	wrapper: {
		position: "relative",
	},
	inputContainer: {
		display: "flex",
		flexWrap: "wrap",
		alignItems: "center",
		gap: tokens.spacingHorizontalXS,
		padding: `${tokens.spacingVerticalXS} ${tokens.spacingHorizontalS}`,
		minHeight: "32px",
		border: `1px solid ${tokens.colorNeutralStroke1}`,
		borderRadius: tokens.borderRadiusMedium,
		backgroundColor: tokens.colorNeutralBackground1,
		cursor: "text",
	},
	inputContainerFocused: {
		border: `1px solid ${tokens.colorCompoundBrandStroke}`,
		borderBottom: `2px solid ${tokens.colorCompoundBrandStroke}`,
	},
	inputContainerError: {
		border: `1px solid ${tokens.colorPaletteRedBorder2}`,
	},
	inputContainerDisabled: {
		backgroundColor: tokens.colorNeutralBackgroundDisabled,
		border: `1px solid ${tokens.colorNeutralStrokeDisabled}`,
		cursor: "not-allowed",
	},
	tag: {
		maxWidth: "200px",
	},
	input: {
		flex: 1,
		minWidth: "120px",
		border: "none",
		outline: "none",
		backgroundColor: "transparent",
		fontSize: tokens.fontSizeBase300,
		fontFamily: tokens.fontFamilyBase,
		color: tokens.colorNeutralForeground1,
	},
	inputDisabled: {
		cursor: "not-allowed",
		color: tokens.colorNeutralForegroundDisabled,
	},
	chevron: {
		color: tokens.colorNeutralForeground3,
		marginLeft: "auto",
		flexShrink: 0,
	},
	dropdown: {
		position: "fixed",
		backgroundColor: tokens.colorNeutralBackground1,
		border: `1px solid ${tokens.colorNeutralStroke1}`,
		borderRadius: tokens.borderRadiusMedium,
		boxShadow: tokens.shadow16,
		zIndex: 10000,
		maxHeight: "300px",
		overflowY: "auto",
	},
	optionList: {
		listStyle: "none",
		margin: "0",
		padding: tokens.spacingVerticalXS,
	},
	option: {
		display: "flex",
		flexDirection: "column",
		padding: `${tokens.spacingVerticalS} ${tokens.spacingHorizontalM}`,
		cursor: "pointer",
		borderRadius: tokens.borderRadiusMedium,
	},
	optionHover: {
		backgroundColor: tokens.colorNeutralBackground1Hover,
	},
	optionName: {
		fontSize: tokens.fontSizeBase300,
		color: tokens.colorNeutralForeground1,
	},
	optionSecondary: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	loading: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingVerticalM,
		justifyContent: "center",
	},
	noResults: {
		padding: tokens.spacingVerticalM,
		textAlign: "center",
		color: tokens.colorNeutralForeground3,
		fontStyle: "italic",
	},
});

export interface TagPickerOption {
	id: string;
	name: string;
	secondary?: string;
	gid?: string;
	recordId?: number;
}

export interface TagPickerProps {
	label: string;
	value: string[];
	onChange: (value: string[], options: TagPickerOption[]) => void;
	loadOptions: (
		query: string,
		ids?: string[],
		limit?: number,
	) => Promise<TagPickerOption[]>;
	placeholder?: string;
	required?: boolean;
	error?: string;
	disabled?: boolean;
	debounceMs?: number;
	initialSuggestionsLimit?: number;
	maxSelectedItems?: number;
}

const TagPicker: React.FC<TagPickerProps> = ({
	label,
	value,
	onChange,
	loadOptions,
	placeholder = "Wyszukaj...",
	required = false,
	error,
	disabled = false,
	debounceMs = 300,
	initialSuggestionsLimit = 25,
	maxSelectedItems,
}) => {
	const styles = useStyles();
	const [inputValue, setInputValue] = useState("");
	const [options, setOptions] = useState<TagPickerOption[]>([]);
	const [selectedOptions, setSelectedOptions] = useState<TagPickerOption[]>([]);
	const [isLoading, setIsLoading] = useState(false);
	const [isOpen, setIsOpen] = useState(false);
	const [isFocused, setIsFocused] = useState(false);
	const [hoveredOptionId, setHoveredOptionId] = useState<string | null>(null);
	const [dropdownPosition, setDropdownPosition] = useState({
		top: 0,
		left: 0,
		width: 0,
	});
	const inputRef = useRef<HTMLInputElement>(null);
	const wrapperRef = useRef<HTMLDivElement>(null);
	const dropdownRef = useRef<HTMLDivElement>(null);
	const lastValueRef = useRef<string[]>([]);

	const debouncedQuery = useDebounce(inputValue, debounceMs);

	// Update dropdown position
	const updateDropdownPosition = useCallback(() => {
		if (wrapperRef.current) {
			const rect = wrapperRef.current.getBoundingClientRect();
			setDropdownPosition({
				top: rect.bottom + 2,
				left: rect.left,
				width: rect.width,
			});
		}
	}, []);

	// Update position when opening or on scroll/resize
	useEffect(() => {
		if (isOpen) {
			updateDropdownPosition();
			window.addEventListener("scroll", updateDropdownPosition, true);
			window.addEventListener("resize", updateDropdownPosition);
			return () => {
				window.removeEventListener("scroll", updateDropdownPosition, true);
				window.removeEventListener("resize", updateDropdownPosition);
			};
		}
	}, [isOpen, updateDropdownPosition]);

	const fetchOptions = useCallback(
		async (query: string, limit?: number) => {
			setIsLoading(true);
			try {
				const results = await loadOptions(query, undefined, limit);
				setOptions(results);
			} catch (err) {
				console.error("Error loading picker options:", err);
				setOptions([]);
			} finally {
				setIsLoading(false);
			}
		},
		[loadOptions],
	);

	// Load suggestions when opening or query changes
	useEffect(() => {
		if (isOpen) {
			fetchOptions(debouncedQuery || "", initialSuggestionsLimit);
		}
	}, [isOpen, debouncedQuery, fetchOptions, initialSuggestionsLimit]);

	// Initialize selected options from value prop
	useEffect(() => {
		const valueStr = JSON.stringify(value);
		const lastValueStr = JSON.stringify(lastValueRef.current);
		if (valueStr === lastValueStr) {
			return;
		}
		lastValueRef.current = value;

		if (value.length === 0) {
			setSelectedOptions([]);
			return;
		}

		loadOptions("", value).then((results) => {
			const matched = results.filter((opt) => value.includes(opt.id));
			setSelectedOptions(matched);
		});
	}, [value, loadOptions]);

	// Close dropdown when clicking outside
	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			const target = event.target as Node;
			if (
				wrapperRef.current &&
				!wrapperRef.current.contains(target) &&
				dropdownRef.current &&
				!dropdownRef.current.contains(target)
			) {
				setIsOpen(false);
				setIsFocused(false);
			}
		};

		document.addEventListener("mousedown", handleClickOutside);
		return () => document.removeEventListener("mousedown", handleClickOutside);
	}, []);

	const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		setInputValue(e.target.value);
		if (!isOpen) {
			setIsOpen(true);
		}
	};

	const handleInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
		if (
			e.key === "Backspace" &&
			inputValue === "" &&
			selectedOptions.length > 0
		) {
			const newSelected = selectedOptions.slice(0, -1);
			setSelectedOptions(newSelected);
			onChange(
				newSelected.map((o) => o.id),
				newSelected,
			);
		} else if (e.key === "Escape") {
			setIsOpen(false);
		} else if (e.key === "ArrowDown" && !isOpen) {
			setIsOpen(true);
		}
	};

	const handleOptionSelect = (option: TagPickerOption) => {
		const isSelected = selectedOptions.some((o) => o.id === option.id);

		let newSelected: TagPickerOption[];
		if (isSelected) {
			newSelected = selectedOptions.filter((o) => o.id !== option.id);
		} else if (maxSelectedItems === 1) {
			newSelected = [option];
		} else if (maxSelectedItems && selectedOptions.length >= maxSelectedItems) {
			return;
		} else {
			newSelected = [...selectedOptions, option];
		}

		setSelectedOptions(newSelected);
		onChange(
			newSelected.map((o) => o.id),
			newSelected,
		);
		setInputValue("");

		if (maxSelectedItems && newSelected.length >= maxSelectedItems) {
			setIsOpen(false);
		} else {
			inputRef.current?.focus();
		}
	};

	const handleTagDismiss = (optionId: string) => {
		const newSelected = selectedOptions.filter((o) => o.id !== optionId);
		setSelectedOptions(newSelected);
		onChange(
			newSelected.map((o) => o.id),
			newSelected,
		);
	};

	const handleContainerClick = () => {
		if (!disabled) {
			inputRef.current?.focus();
			setIsOpen(true);
		}
	};

	const handleInputFocus = () => {
		setIsFocused(true);
		setIsOpen(true);
	};

	const filteredOptions = options.filter(
		(opt) => !selectedOptions.some((sel) => sel.id === opt.id),
	);

	const canAddMore =
		!maxSelectedItems || selectedOptions.length < maxSelectedItems;

	const dropdownContent = isOpen && !disabled && (
		<div
			ref={dropdownRef}
			className={styles.dropdown}
			style={{
				top: dropdownPosition.top,
				left: dropdownPosition.left,
				width: dropdownPosition.width,
			}}
		>
			{isLoading ? (
				<div className={styles.loading}>
					<Spinner size="tiny" />
					<span>Wczytywanie...</span>
				</div>
			) : filteredOptions.length === 0 ? (
				<div className={styles.noResults}>
					{inputValue ? "Brak wyników" : "Brak dostępnych opcji"}
				</div>
			) : (
				<ul className={styles.optionList}>
					{filteredOptions.map((option) => (
						<li
							key={option.id}
							className={mergeClasses(
								styles.option,
								hoveredOptionId === option.id && styles.optionHover,
							)}
							onClick={() => handleOptionSelect(option)}
							onMouseEnter={() => setHoveredOptionId(option.id)}
							onMouseLeave={() => setHoveredOptionId(null)}
						>
							<span className={styles.optionName}>{option.name}</span>
							{option.secondary && (
								<span className={styles.optionSecondary}>
									{option.secondary}
								</span>
							)}
						</li>
					))}
				</ul>
			)}
		</div>
	);

	return (
		<Field
			label={required ? `${label} *` : label}
			validationState={error ? "error" : "none"}
			validationMessage={error}
		>
			<div ref={wrapperRef} className={styles.wrapper}>
				<div
					className={mergeClasses(
						styles.inputContainer,
						isFocused && !disabled && styles.inputContainerFocused,
						error && styles.inputContainerError,
						disabled && styles.inputContainerDisabled,
					)}
					onClick={handleContainerClick}
				>
					{selectedOptions.map((option) => (
						<Tag
							key={option.id}
							className={styles.tag}
							dismissible={!disabled}
							dismissIcon={{ "aria-label": "Usuń" }}
							onClick={(e) => {
								e.stopPropagation();
								if (!disabled) {
									handleTagDismiss(option.id);
								}
							}}
							size="small"
						>
							{option.name}
						</Tag>
					))}
					{canAddMore && (
						<input
							ref={inputRef}
							type="text"
							className={mergeClasses(
								styles.input,
								disabled && styles.inputDisabled,
							)}
							value={inputValue}
							onChange={handleInputChange}
							onKeyDown={handleInputKeyDown}
							placeholder={selectedOptions.length === 0 ? placeholder : ""}
							disabled={disabled}
							onFocus={handleInputFocus}
						/>
					)}
					<ChevronDownRegular className={styles.chevron} />
				</div>

				<Portal>{dropdownContent}</Portal>
			</div>
		</Field>
	);
};

export default TagPicker;
