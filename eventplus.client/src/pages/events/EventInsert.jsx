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
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Paper,
	Dialog,
	DialogTitle,
	DialogContent,
	DialogActions,
	Tooltip,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import {
	ArrowBack as ArrowBackIcon,
	Save as SaveIcon,
	Add as AddIcon,
	Delete as DeleteIcon,
	Edit as EditIcon,
	GridView as GridViewIcon,
	Chair as ChairIcon,
} from "@mui/icons-material";
import { fetchCategories } from "../../services/categoryService";

import LoadingSpinner from "../../components/shared/LoadingSpinner";
import ErrorDisplay from "../../components/shared/ErrorDisplay";
import ToastNotification from "../../components/shared/ToastNotification";
import { useAuth } from "../../context/AuthContext";

function EventInsert() {
	const navigate = useNavigate();
	
	const [sectorDialogOpen, setSectorDialogOpen] = useState(false);
	const [priceDialogOpen, setPriceDialogOpen] = useState(false);
	const [seatingDialogOpen, setSeatingDialogOpen] = useState(false);
	const [bulkDialogOpen, setBulkDialogOpen] = useState(false);

	const [currentSector, setCurrentSector] = useState({name: ''});
	const [currentPrice, setCurrentPrice] = useState({sectorId: '', price: 0});
	const [currentSeating, setCurrentSeating] = useState({sectorId: '', row: '', place: ''});
	const [bulkSeatData, setBulkSeatData] = useState({sectorId: '', rows: 1, seatsPerRow: 1});

	const [editingSector, setEditingSector] = useState(false);
	const [editingPrice, setEditingPrice] = useState(false);
	const [editingSeating, setEditingSeating] = useState(false);


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
	const { currentUser } = useAuth();

	
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
			fkOrganiseridUser: currentUser.id,
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
		sectors: [],
		sectorPrices: [],
		seatings: [],
	});

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

	const handleAddSector = () => {
		setCurrentSector({name: ''});
		setEditingSector(false);
		setSectorDialogOpen(true);
	};
	
	const handleEditSector = (sector) => {
		setCurrentSector({...sector});
		setEditingSector(true);
		setSectorDialogOpen(true);
	};
	
	const handleSaveSector = () => {
		if (editingSector) {
			const updatedSectors = formData.sectors.map(s => 
				(s.id === currentSector.id || s.tempId === currentSector.tempId) ? currentSector : s
			);
			setFormData({...formData, sectors: updatedSectors});
		} else {
			const newSector = {
				...currentSector,
				tempId: `temp-${Date.now()}`
			};
			setFormData({...formData, sectors: [...formData.sectors, newSector]});
		}
		setSectorDialogOpen(false);
	};
	
	const handleDeleteSector = (sectorId) => {
		// Remove the sector and any related prices/seats
		const updatedSectors = formData.sectors.filter(s => 
			s.id !== sectorId && s.tempId !== sectorId
		);
		
		const updatedPrices = formData.sectorPrices.filter(p => 
			p.sectorId !== sectorId
		);
		
		const updatedSeatings = formData.seatings.filter(seat => 
			seat.sectorId !== sectorId
		);
		
		setFormData({
			...formData, 
			sectors: updatedSectors,
			sectorPrices: updatedPrices,
			seatings: updatedSeatings
		});
	};

	const handleAddSectorPrice = () => {
		setCurrentPrice({sectorId: formData.sectors[0]?.id || formData.sectors[0]?.tempId || '', price: 0});
		setEditingPrice(false);
		setPriceDialogOpen(true);
	};
	
	const handleEditSectorPrice = (price) => {
		setCurrentPrice({...price});
		setEditingPrice(true);
		setPriceDialogOpen(true);
	};
	
	const handleSavePrice = () => {
		if (editingPrice) {
			const updatedPrices = formData.sectorPrices.map(p => 
				(p.id === currentPrice.id || p.tempId === currentPrice.tempId) ? currentPrice : p
			);
			setFormData({...formData, sectorPrices: updatedPrices});
		} else {
			const newPrice = {
				...currentPrice,
				tempId: `temp-${Date.now()}`
			};
			setFormData({...formData, sectorPrices: [...formData.sectorPrices, newPrice]});
		}
		setPriceDialogOpen(false);
	};
	
	const handleDeleteSectorPrice = (priceId) => {
		const updatedPrices = formData.sectorPrices.filter(p => 
			p.id !== priceId && p.tempId !== priceId
		);
		setFormData({...formData, sectorPrices: updatedPrices});
	};
	
	const handleAddSeating = () => {
		setCurrentSeating({
			sectorId: formData.sectors[0]?.id || formData.sectors[0]?.tempId || '', 
			row: 1, 
			place: 1
		});
		setEditingSeating(false);
		setSeatingDialogOpen(true);
	};
	
	const handleEditSeating = (seat) => {
		setCurrentSeating({...seat});
		setEditingSeating(true);
		setSeatingDialogOpen(true);
	};
	
	const handleSaveSeating = () => {
		if (editingSeating) {
			const updatedSeatings = formData.seatings.map(s => 
				(s.id === currentSeating.id || s.tempId === currentSeating.tempId) ? currentSeating : s
			);
			setFormData({...formData, seatings: updatedSeatings});
		} else {
			const newSeating = {
				...currentSeating,
				tempId: `temp-${Date.now()}`
			};
			setFormData({...formData, seatings: [...formData.seatings, newSeating]});
		}
		setSeatingDialogOpen(false);
	};
	
	const handleDeleteSeating = (seatId) => {
		const updatedSeatings = formData.seatings.filter(s => 
			s.id !== seatId && s.tempId !== seatId
		);
		setFormData({...formData, seatings: updatedSeatings});
	};
	
	const handleBulkAddSeats = () => {
		setBulkSeatData({
			sectorId: formData.sectors[0]?.id || formData.sectors[0]?.tempId || '',
			rows: 5,
			seatsPerRow: 10
		});
		setBulkDialogOpen(true);
	};
	
	const handleBulkSeatsSave = () => {
		const { sectorId, rows, seatsPerRow } = bulkSeatData;
		const newSeats = [];
		
		for (let row = 1; row <= rows; row++) {
			for (let place = 1; place <= seatsPerRow; place++) {
				newSeats.push({
					sectorId,
					row,
					place,
					tempId: `temp-${Date.now()}-${row}-${place}`
				});
			}
		}
		
		setFormData({
			...formData,
			seatings: [...formData.seatings, ...newSeats]
		});
		
		setBulkDialogOpen(false);
	};

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
			sectors: formData.sectors.map(sector => ({
			  name: sector.name,
			  id: sector.id || 0,
			  tempId: sector.tempId // Pass the tempId for reference
			})),
			sectorPrices: formData.sectorPrices.map(price => ({
			  price: parseFloat(price.price),
			  sectorId: price.sectorId, // Keep the original sectorId (real ID or tempId)
			  id: price.id || 0
			})),
			seatings: formData.seatings.map(seat => ({
			  row: parseInt(seat.row, 10),
			  place: parseInt(seat.place, 10),
			  sectorId: seat.sectorId, // Keep the original sectorId (real ID or tempId)
			  id: seat.id || 0
			}))
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

	const handleCloseToast = () => {
		setToast({ ...toast, open: false });
	};

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
						<Tab label="Seating" />
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

						{/* Seating Tab */}
						{activeTab === 4 && (
							<Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
								{/* Sectors Management Section */}
								<Box>
									<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
										<Typography variant="h6">Sectors</Typography>
										<Button 
											variant="contained" 
											size="small" 
											startIcon={<AddIcon />}
											onClick={handleAddSector}
										>
											Add Sector
										</Button>
									</Box>
									
									<TableContainer component={Paper} sx={{ mb: 3 }}>
										<Table size="small">
											<TableHead>
												<TableRow>
													<TableCell>Name</TableCell>
													<TableCell align="right">Actions</TableCell>
												</TableRow>
											</TableHead>
											<TableBody>
												{formData.sectors.length > 0 ? (
													formData.sectors.map((sector) => (
														<TableRow key={sector.id || sector.tempId}>
															<TableCell>{sector.name}</TableCell>
															<TableCell align="right">
																<Tooltip title="Edit">
																	<IconButton size="small" onClick={() => handleEditSector(sector)}>
																		<EditIcon fontSize="small" />
																	</IconButton>
																</Tooltip>
																<Tooltip title="Delete">
																	<IconButton 
																		size="small" 
																		color="error"
																		onClick={() => handleDeleteSector(sector.id || sector.tempId)}
																	>
																		<DeleteIcon fontSize="small" />
																	</IconButton>
																</Tooltip>
															</TableCell>
														</TableRow>
													))
												) : (
													<TableRow>
														<TableCell colSpan={2} align="center">No sectors added</TableCell>
													</TableRow>
												)}
											</TableBody>
										</Table>
									</TableContainer>
								</Box>
								
								{/* Sector Prices Section */}
								<Box>
									<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
										<Typography variant="h6">Sector Prices</Typography>
										<Button 
											variant="contained" 
											size="small" 
											startIcon={<AddIcon />}
											onClick={handleAddSectorPrice}
											disabled={formData.sectors.length === 0}
										>
											Add Price
										</Button>
									</Box>
									
									<TableContainer component={Paper} sx={{ mb: 3 }}>
										<Table size="small">
											<TableHead>
												<TableRow>
													<TableCell>Sector</TableCell>
													<TableCell>Price</TableCell>
													<TableCell align="right">Actions</TableCell>
												</TableRow>
											</TableHead>
											<TableBody>
												{formData.sectorPrices.length > 0 ? (
													formData.sectorPrices.map((price) => (
														<TableRow key={price.id || price.tempId}>
															<TableCell>
																{formData.sectors.find(s => s.id === price.sectorId || s.tempId === price.sectorId)?.name || 'Unknown'}
															</TableCell>
															<TableCell>${price.price.toFixed(2)}</TableCell>
															<TableCell align="right">
																<Tooltip title="Edit">
																	<IconButton size="small" onClick={() => handleEditSectorPrice(price)}>
																		<EditIcon fontSize="small" />
																	</IconButton>
																</Tooltip>
																<Tooltip title="Delete">
																	<IconButton 
																		size="small" 
																		color="error"
																		onClick={() => handleDeleteSectorPrice(price.id || price.tempId)}
																	>
																		<DeleteIcon fontSize="small" />
																	</IconButton>
																</Tooltip>
															</TableCell>
														</TableRow>
													))
												) : (
													<TableRow>
														<TableCell colSpan={3} align="center">No prices added</TableCell>
													</TableRow>
												)}
											</TableBody>
										</Table>
									</TableContainer>
								</Box>
								
								{/* Seats Management */}
								<Box>
									<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
										<Typography variant="h6">Seats</Typography>
										<Box>
											<Button 
												variant="contained" 
												size="small" 
												startIcon={<AddIcon />}
												onClick={handleAddSeating}
												disabled={formData.sectors.length === 0}
												sx={{ mr: 1 }}
											>
												Add Seat
											</Button>
											<Button
												variant="outlined"
												size="small"
												startIcon={<ChairIcon />}
												onClick={handleBulkAddSeats}
												disabled={formData.sectors.length === 0}
											>
												Bulk Add
											</Button>
										</Box>
									</Box>
									
									<TableContainer component={Paper}>
										<Table size="small">
											<TableHead>
												<TableRow>
													<TableCell>Sector</TableCell>
													<TableCell>Row</TableCell>
													<TableCell>Place</TableCell>
													<TableCell>Place in Sector</TableCell>
													<TableCell align="right">Actions</TableCell>
												</TableRow>
											</TableHead>
											<TableBody>
												{formData.seatings.length > 0 ? (
													formData.seatings.map((seat) => (
														<TableRow key={seat.id || seat.tempId}>
															<TableCell>
																{formData.sectors.find(s => s.id === seat.sectorId || s.tempId === seat.sectorId)?.name || 'Unknown'}
															</TableCell>
															<TableCell>{seat.row}</TableCell>
															<TableCell>{seat.place}</TableCell>
															<TableCell align="right">
																<Tooltip title="Edit">
																	<IconButton size="small" onClick={() => handleEditSeating(seat)}>
																		<EditIcon fontSize="small" />
																	</IconButton>
																</Tooltip>
																<Tooltip title="Delete">
																	<IconButton 
																		size="small" 
																		color="error"
																		onClick={() => handleDeleteSeating(seat.id || seat.tempId)}
																	>
																		<DeleteIcon fontSize="small" />
																	</IconButton>
																</Tooltip>
															</TableCell>
														</TableRow>
													))
												) : (
													<TableRow>
														<TableCell colSpan={5} align="center">No seats added</TableCell>
													</TableRow>
												)}
											</TableBody>
										</Table>
									</TableContainer>
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

							{activeTab < 5 ? (
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

			{/* Sector Dialog */}
			<Dialog open={sectorDialogOpen} onClose={() => setSectorDialogOpen(false)}>
				<DialogTitle>{editingSector ? 'Edit Sector' : 'Add Sector'}</DialogTitle>
				<DialogContent>
					<TextField
						autoFocus
						margin="dense"
						label="Sector Name"
						fullWidth
						value={currentSector.name}
						onChange={(e) => setCurrentSector({...currentSector, name: e.target.value})}
					/>
				</DialogContent>
				<DialogActions>
					<Button onClick={() => setSectorDialogOpen(false)}>Cancel</Button>
					<Button onClick={handleSaveSector} variant="contained">Save</Button>
				</DialogActions>
			</Dialog>

			{/* Sector Price Dialog */}
			<Dialog open={priceDialogOpen} onClose={() => setPriceDialogOpen(false)}>
				<DialogTitle>{editingPrice ? 'Edit Price' : 'Add Price'}</DialogTitle>
				<DialogContent>
					<FormControl fullWidth margin="dense">
						<InputLabel>Sector</InputLabel>
						<Select
							value={currentPrice.sectorId}
							onChange={(e) => setCurrentPrice({...currentPrice, sectorId: e.target.value})}
							label="Sector"
						>
							{formData.sectors.map((sector) => (
								<MenuItem key={sector.id || sector.tempId} value={sector.id || sector.tempId}>
									{sector.name}
								</MenuItem>
							))}
						</Select>
					</FormControl>
					<TextField
						margin="dense"
						label="Price"
						type="number"
						fullWidth
						value={currentPrice.price}
						onChange={(e) => setCurrentPrice({...currentPrice, price: Number(e.target.value)})}
					/>
				</DialogContent>
				<DialogActions>
					<Button onClick={() => setPriceDialogOpen(false)}>Cancel</Button>
					<Button onClick={handleSavePrice} variant="contained">Save</Button>
				</DialogActions>
			</Dialog>

			{/* Seating Dialog */}
			<Dialog open={seatingDialogOpen} onClose={() => setSeatingDialogOpen(false)}>
				<DialogTitle>{editingSeating ? 'Edit Seat' : 'Add Seat'}</DialogTitle>
				<DialogContent>
					<FormControl fullWidth margin="dense">
						<InputLabel>Sector</InputLabel>
						<Select
							value={currentSeating.sectorId}
							onChange={(e) => setCurrentSeating({...currentSeating, sectorId: e.target.value})}
							label="Sector"
						>
							{formData.sectors.map((sector) => (
								<MenuItem key={sector.id || sector.tempId} value={sector.id || sector.tempId}>
									{sector.name}
								</MenuItem>
							))}
						</Select>
					</FormControl>
					<TextField
						margin="dense"
						label="Row"
						type="number"
						fullWidth
						value={currentSeating.row}
						onChange={(e) => setCurrentSeating({...currentSeating, row: Number(e.target.value)})}
					/>
					<TextField
						margin="dense"
						label="Place"
						type="number"
						fullWidth
						value={currentSeating.place}
						onChange={(e) => setCurrentSeating({...currentSeating, place: Number(e.target.value)})}
					/>
				</DialogContent>
				<DialogActions>
					<Button onClick={() => setSeatingDialogOpen(false)}>Cancel</Button>
					<Button onClick={handleSaveSeating} variant="contained">Save</Button>
				</DialogActions>
			</Dialog>

			{/* Bulk Add Seats Dialog */}
			<Dialog open={bulkDialogOpen} onClose={() => setBulkDialogOpen(false)}>
				<DialogTitle>Bulk Add Seats</DialogTitle>
				<DialogContent>
					<FormControl fullWidth margin="dense">
						<InputLabel>Sector</InputLabel>
						<Select
							value={bulkSeatData.sectorId}
							onChange={(e) => setBulkSeatData({...bulkSeatData, sectorId: e.target.value})}
							label="Sector"
						>
							{formData.sectors.map((sector) => (
								<MenuItem key={sector.id || sector.tempId} value={sector.id || sector.tempId}>
									{sector.name}
								</MenuItem>
							))}
						</Select>
					</FormControl>
					<TextField
						margin="dense"
						label="Number of Rows"
						type="number"
						fullWidth
						value={bulkSeatData.rows}
						onChange={(e) => setBulkSeatData({...bulkSeatData, rows: Number(e.target.value)})}
					/>
					<TextField
						margin="dense"
						label="Seats per Row"
						type="number"
						fullWidth
						value={bulkSeatData.seatsPerRow}
						onChange={(e) => setBulkSeatData({...bulkSeatData, seatsPerRow: Number(e.target.value)})}
					/>
				</DialogContent>
				<DialogActions>
					<Button onClick={() => setBulkDialogOpen(false)}>Cancel</Button>
					<Button onClick={handleBulkSeatsSave} variant="contained">Create</Button>
				</DialogActions>
			</Dialog>

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
