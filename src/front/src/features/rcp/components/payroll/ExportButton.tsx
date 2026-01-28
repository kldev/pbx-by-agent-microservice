import { Button } from "@fluentui/react-components";
import { ArrowDownloadRegular } from "@fluentui/react-icons";
import type React from "react";
import { useState } from "react";

const API_BASE_URL = import.meta.env.VITE_API_URL ?? "";

interface ExportButtonProps {
	year: number;
	month: number;
}

const ExportButton: React.FC<ExportButtonProps> = ({ year, month }) => {
	const [isExporting, setIsExporting] = useState(false);

	const handleExport = async () => {
		setIsExporting(true);
		try {
			const response = await fetch(`${API_BASE_URL}/api/rcp/payroll/export`, {
				method: "POST",
				credentials: "include",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify({ year, month }),
			});

			if (!response.ok) {
				throw new Error("Export failed");
			}

			const blob = await response.blob();
			const url = window.URL.createObjectURL(blob);
			const link = document.createElement("a");
			link.href = url;
			link.download = `RCP_${year}-${month.toString().padStart(2, "0")}.xlsx`;
			document.body.appendChild(link);
			link.click();
			document.body.removeChild(link);
			window.URL.revokeObjectURL(url);
		} catch (error) {
			console.error("Export failed:", error);
		} finally {
			setIsExporting(false);
		}
	};

	return (
		<Button
			appearance="secondary"
			icon={<ArrowDownloadRegular />}
			onClick={handleExport}
			disabled={isExporting}
		>
			{isExporting ? "Eksportowanie..." : "Pobierz Excel"}
		</Button>
	);
};

export default ExportButton;
