import {
	CardHeader,
	Card as FluentCard,
	type CardProps as FluentCardProps,
	makeStyles,
	Text,
	tokens,
} from "@fluentui/react-components";
import type React from "react";

const useStyles = makeStyles({
	card: {
		padding: tokens.spacingVerticalM,
	},
	header: {
		paddingBottom: tokens.spacingVerticalS,
	},
	title: {
		fontWeight: tokens.fontWeightSemibold,
	},
	content: {
		padding: tokens.spacingVerticalS,
	},
});

export interface CardProps extends Omit<FluentCardProps, "title"> {
	title?: string;
	description?: string;
	headerAction?: React.ReactNode;
	children: React.ReactNode;
}

const Card: React.FC<CardProps> = ({
	title,
	description,
	headerAction,
	children,
	className,
	...props
}) => {
	const styles = useStyles();

	return (
		<FluentCard {...props} className={`${styles.card} ${className || ""}`}>
			{(title || headerAction) && (
				<CardHeader
					className={styles.header}
					header={
						title ? <Text className={styles.title}>{title}</Text> : undefined
					}
					description={description ? <Text>{description}</Text> : undefined}
					action={headerAction ? <>{headerAction}</> : undefined}
				/>
			)}
			<div className={styles.content}>{children}</div>
		</FluentCard>
	);
};

export default Card;
