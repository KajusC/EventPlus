import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createEvent } from "../../services/eventService";
import {
	Container,
	Typography,
	Button,
	Box,
	Card,
	CardContent,
	TextField,
	FormControl,
	InputLabel,
	Select,
	MenuItem,
	Tabs,
	Tab,
	IconButton,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import {
	ArrowBack as ArrowBackIcon,
	Save as SaveIcon,
} from "@mui/icons-material";
import { fetchCategories } from "../../services/categoryService";

// Import shared components
import LoadingSpinner from "../../components/shared/LoadingSpinner";
import ErrorDisplay from "../../components/shared/ErrorDisplay";
import ToastNotification from "../../components/shared/ToastNotification";

function EventInsert() {
	const navigate = useNavigate();
	
	// Simplified state management
	const [isLoading, setIsLoading] = useState(false);
	const [isSubmitting, setIsSubmitting] = useState(false);
	const [error, setError] = useState(null);
	const [categories, setCategories] = useState([]);
	const [activeTab, setActiveTab] = useState(0);
	const [toast, setToast] = useState({
		open: false,
		message: "",
		severity: "info"
	});
	
	// Form data state
	const [formData, setFormData] = useState({
		event: {
			idEvent: 0,
			name: "",
			description: "",
			startDate: new Date(),
			endDate: new Date(new Date().getTime() + 2 * 60 * 60 * 1000),
			maxTicketCount: 0,
			category: 1,
			fkEventLocationidEventLocation: 0,
			fkOrganiseridUser: 1, // This would ideally come from auth context
		},
		eventLocation: {
			name: "",
			address: "",
			city: "",
			country: "",
			capacity: 0,
			contacts: "",
			price: 0,
			idEventLocation: 0,
			idEquipment: 0,
			sectorIds: [0],
		},
		partners: {
			name: "",
			description: "",
			website: "",
			idPartner: 0,
		},
		performers: {
			name: "",
			surname: "",
			profession: "",
			idPerformer: 0,
		},
	});

	// Load categories
	useEffect(() => {
		const fetchCategory = async () => {
			try {
				setIsLoading(true);
				const data = await fetchCategories();
				setCategories(data);
				if (data && data.length > 0) {
					setFormData((prev) => ({
						...prev,
						event: {
							...prev.event,
							category: data[0].idCategory,
						},
					}));
				}
				setError(null);
			} catch (error) {
				console.error("Error fetching categories:", error);
				setError("Failed to load categories. Please try again later.");
			} finally {
				setIsLoading(false);
			}
		};
		fetchCategory();
	}, []);

	// Form input handlers
	const handleInputChange = (section, field, value) => {
		setFormData((prev) => ({
			...prev,
			[section]: {
				...prev[section],
				[field]: value,
			},
		}));
	};

	const handleDateChange = (section, field) => (newDate) => {
		setFormData((prev) => ({
			...prev,
			[section]: {
				...prev[section],
				[field]: newDate,
			},
		}));
	};

	const handleTabChange = (event, newValue) => {
		setActiveTab(newValue);
	};

	// Form submission
	const handleSubmit = async (e) => {
		e.preventDefault();

		try {
			setIsSubmitting(true);
			const dataToSubmit = {
				event: {
					...formData.event,
					startDate: formData.event.startDate.toISOString().split("T")[0],
					endDate: formData.event.endDate.toISOString().split("T")[0],
					maxTicketCount: parseInt(formData.event.maxTicketCount, 10),
					category: parseInt(formData.event.category, 10),
				},
				eventLocation: {
					...formData.eventLocation,
					capacity: parseInt(formData.eventLocation.capacity, 10),
					price: parseFloat(formData.eventLocation.price),
					idEquipment: parseInt(
						formData.eventLocation.idEquipment,
						10
					),
				},
				partners: formData.partners,
				performers: formData.performers,
			};

			console.log("Submitting data:", JSON.stringify(dataToSubmit, null, 2));
			await createEvent(dataToSubmit);
			
			setToast({
				open: true,
				message: "Event created successfully!",
				severity: "success"
			});
			
			setTimeout(() => {
				navigate("/events");
			}, 1500);
		} catch (err) {
			setError(err.message || "Failed to create event");
			
			setToast({
				open: true,
				message: `Error: ${err.message || "Failed to create event"}`,
				severity: "error"
			});
			
			console.error("Create error:", err);
		} finally {
			setIsSubmitting(false);
		}
	};

	// Handle toast close
	const handleCloseToast = () => {
		setToast({ ...toast, open: false });
	};

	// Early return for loading and error states
	if (isLoading) return <LoadingSpinner />;
	if (error && !toast.open) return <ErrorDisplay error={error} />;

	return (
		<Container maxWidth="lg" sx={{ mt: 8, mb: 8 }}>
			<Box sx={{ display: "flex", alignItems: "center", mb: 4, pt: 4 }}>
				<IconButton
					onClick={() => navigate("/events")}
					sx={{ mr: 2, color: 'primary.main' }}
					aria-label="go back"
				>
					<ArrowBackIcon />
				</IconButton>
				<Typography variant="h4" component="h1">
					Create New Event
				</Typography>
			</Box>

			<Card sx={{ mb: 4, borderRadius: 2, boxShadow: '0 8px 40px rgba(0,0,0,0.12)' }}>
				<CardContent>
					<Tabs
						value={activeTab}
						onChange={handleTabChange}
						sx={{ mb: 3, borderBottom: 1, borderColor: "divider" }}
					>
						<Tab label="Event Details" />
						<Tab label="Location" />
						<Tab label="Partner" />
						<Tab label="Performer" />
					</Tabs>

					<form onSubmit={handleSubmit}>
						{/* Event Details Tab */}
						{activeTab === 0 && (
							<Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Event Name"
										value={formData.event.name}
										onChange={(e) =>
											handleInputChange("event", "name", e.target.value)
										}
										required
									/>
								</Box>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Description"
										value={formData.event.description}
										onChange={(e) =>
											handleInputChange("event", "description", e.target.value)
										}
										multiline
										rows={4}
										required
									/>
								</Box>
								<Box sx={{ display: 'flex', gap: 3 }}>
									<Box sx={{ width: "50%" }}>
										<FormControl fullWidth>
											<InputLabel>Category</InputLabel>
											<Select
												value={formData.event.category}
												onChange={(e) =>
													handleInputChange("event", "category", e.target.value)
												}
												label="Category"
												required
											>
												{categories.map((category) => (
													<MenuItem
														key={category.idCategory}
														value={category.idCategory}
													>
														{category.name}
													</MenuItem>
												))}
											</Select>
										</FormControl>
									</Box>
									<Box sx={{ width: "50%" }}>
										<TextField
											fullWidth
											type="number"
											label="Max Ticket Count"
											value={formData.event.maxTicketCount}
											onChange={(e) =>
												handleInputChange(
													"event",
													"maxTicketCount",
													e.target.value
												)
											}
											required
										/>
									</Box>
								</Box>
								<Box sx={{ display: 'flex', gap: 3 }}>
									<Box sx={{ width: "50%" }}>
										<LocalizationProvider dateAdapter={AdapterDateFns}>
											<DatePicker
												label="Start Date"
												value={formData.event.startDate}
												onChange={handleDateChange("event", "startDate")}
												slotProps={{
													textField: { fullWidth: true, required: true },
												}}
											/>
										</LocalizationProvider>
									</Box>
									<Box sx={{ width: "50%" }}>
										<LocalizationProvider dateAdapter={AdapterDateFns}>
											<DatePicker
												label="End Date"
												value={formData.event.endDate}
												onChange={handleDateChange("event", "endDate")}
												minDate={formData.event.startDate}
												slotProps={{
													textField: { fullWidth: true, required: true },
												}}
											/>
										</LocalizationProvider>
									</Box>
								</Box>
							</Box>
						)}

						{/* Location Tab */}
						{activeTab === 1 && (
							<Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Location Name"
										value={formData.eventLocation.name}
										onChange={(e) =>
											handleInputChange("eventLocation", "name", e.target.value)
										}
										required
									/>
								</Box>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Address"
										value={formData.eventLocation.address}
										onChange={(e) =>
											handleInputChange(
												"eventLocation",
												"address",
												e.target.value
											)
										}
										required
									/>
								</Box>
								<Box sx={{ display: 'flex', gap: 3 }}>
									<Box sx={{ width: "50%" }}>
										<TextField
											fullWidth
											label="City"
											value={formData.eventLocation.city}
											onChange={(e) =>
												handleInputChange("eventLocation", "city", e.target.value)
											}
											required
										/>
									</Box>
									<Box sx={{ width: "50%" }}>
										<TextField
											fullWidth
											label="Country"
											value={formData.eventLocation.country}
											onChange={(e) =>
												handleInputChange(
													"eventLocation",
													"country",
													e.target.value
												)
											}
											required
										/>
									</Box>
								</Box>
								<Box sx={{ display: 'flex', gap: 3 }}>
									<Box sx={{ width: "33.33%" }}>
										<TextField
											fullWidth
											type="number"
											label="Capacity"
											value={formData.eventLocation.capacity}
											onChange={(e) =>
												handleInputChange(
													"eventLocation",
													"capacity",
													e.target.value
												)
											}
											required
										/>
									</Box>
									<Box sx={{ width: "33.33%" }}>
										<TextField
											fullWidth
											type="number"
											label="Price"
											value={formData.eventLocation.price}
											onChange={(e) =>
												handleInputChange(
													"eventLocation",
													"price",
													e.target.value
												)
											}
											required
										/>
									</Box>
									<Box sx={{ width: "33.33%" }}>
										<TextField
											fullWidth
											type="number"
											label="Equipment ID"
											value={formData.eventLocation.idEquipment}
											onChange={(e) =>
												handleInputChange(
													"eventLocation",
													"idEquipment",
													e.target.value
												)
											}
										/>
									</Box>
								</Box>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Contacts"
										value={formData.eventLocation.contacts}
										onChange={(e) =>
											handleInputChange(
												"eventLocation",
												"contacts",
												e.target.value
											)
										}
									/>
								</Box>
							</Box>
						)}

						{/* Partners Tab */}
						{activeTab === 2 && (
							<Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Partner Name"
										value={formData.partners.name}
										onChange={(e) =>
											handleInputChange("partners", "name", e.target.value)
										}
									/>
								</Box>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Description"
										value={formData.partners.description}
										onChange={(e) =>
											handleInputChange(
												"partners",
												"description",
												e.target.value
											)
										}
										multiline
										rows={3}
									/>
								</Box>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Website"
										value={formData.partners.website}
										onChange={(e) =>
											handleInputChange("partners", "website", e.target.value)
										}
									/>
								</Box>
							</Box>
						)}

						{/* Performers Tab */}
						{activeTab === 3 && (
							<Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
								<Box sx={{ display: 'flex', gap: 3 }}>
									<Box sx={{ width: "50%" }}>
										<TextField
											fullWidth
											label="First Name"
											value={formData.performers.name}
											onChange={(e) =>
												handleInputChange("performers", "name", e.target.value)
											}
										/>
									</Box>
									<Box sx={{ width: "50%" }}>
										<TextField
											fullWidth
											label="Last Name"
											value={formData.performers.surname}
											onChange={(e) =>
												handleInputChange("performers", "surname", e.target.value)
											}
										/>
									</Box>
								</Box>
								<Box sx={{ width: "100%" }}>
									<TextField
										fullWidth
										label="Profession"
										value={formData.performers.profession}
										onChange={(e) =>
											handleInputChange(
												"performers",
												"profession",
												e.target.value
											)
										}
									/>
								</Box>
							</Box>
						)}

						<Box
							sx={{ display: "flex", justifyContent: "space-between", mt: 4 }}
						>
							{activeTab > 0 ? (
								<Button
									variant="outlined"
									onClick={() => setActiveTab(activeTab - 1)}
								>
									Previous
								</Button>
							) : (
								<Box />
							)}

							{activeTab < 3 ? (
								<Button
									variant="contained"
									onClick={() => setActiveTab(activeTab + 1)}
								>
									Next
								</Button>
							) : (
								<Button
									type="submit"
									variant="contained"
									color="primary"
									disabled={isSubmitting}
									startIcon={isSubmitting ? <LoadingSpinner size={16} /> : <SaveIcon />}
								>
									{isSubmitting ? "Creating..." : "Create Event"}
								</Button>
							)}
						</Box>
					</form>
				</CardContent>
			</Card>

			{/* Using our shared toast component */}
			<ToastNotification
				open={toast.open}
				message={toast.message}
				severity={toast.severity}
				onClose={handleCloseToast}
			/>
		</Container>
	);
}

export default EventInsert;
