import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { fetchEventById, updateEvent } from "../../services/eventService";
import {
  Container,
  Typography,
  Button,
  Box,
  CircularProgress,
  Alert,
  Grid,
  Card,
  CardContent,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Stack,
  Snackbar,
  Alert as MuiAlert,
} from "@mui/material";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { Edit as EditIcon, Save as SaveIcon } from "@mui/icons-material";

function EventEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    category: 0,
    startDate: new Date(),
    endDate: new Date(),
    maxTicketCount: 0,
  });
  const [toast, setToast] = useState({
    open: false,
    message: "",
    severity: "success",
  });

  // Categories options
  const categories = [1, 2];

  useEffect(() => {
    const fetchEventData = async () => {
      try {
        const data = await fetchEventById(id);
        setFormData({
          ...data,
          startDate: new Date(data.startDate),
          endDate: new Date(data.endDate),
        });
      } catch (err) {
        setError(err.message || "Failed to load event");
      } finally {
        setLoading(false);
      }
    };

    fetchEventData();
  }, [id]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleDateChange = (name) => (newDate) => {
    setFormData((prev) => ({
      ...prev,
      [name]: newDate,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      setLoading(true);
      const updatedData = {
        ...formData,
        maxTicketCount: parseInt(formData.maxTicketCount, 10),
        category: parseInt(formData.category, 10),
      };

      const response = await updateEvent(id, updatedData);
      if (response === true) {
        setToast({
          open: true,
          message: "Event updated successfully!",
          severity: "success",
        });
        setTimeout(() => {
          navigate(`/eventview/${id}`);
        }, 1500);
      } else {
        setToast({
          open: true,
          message: `Failed to update event: ${response.statusText}`,
          severity: "error",
        });
      }
    } catch (err) {
      setError(err.message || "Failed to update event");
      setToast({
        open: true,
        message: "Failed to update event",
        severity: "error",
      });
      console.error("Update error:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleCloseToast = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }
    setToast({ ...toast, open: false });
  };

  if (loading && !formData.name) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="80vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 8, mb: 4, minHeight: "80vh" }}>
      <Card
        sx={{
          borderRadius: 4,
          boxShadow: "0 8px 40px -12px rgba(0,0,0,0.2)",
          overflow: "hidden",
        }}
      >
        <Box
          sx={{
            height: "120px",
            bgcolor: "#030027",
            position: "relative",
            display: "flex",
            alignItems: "flex-end",
          }}
        >
          <Typography
            variant="h4"
            sx={{
              color: "white",
              p: 3,
              pb: 2,
              textShadow: "1px 1px 3px rgba(0,0,0,0.3)",
              fontWeight: "bold",
              display: "flex",
              alignItems: "center",
              gap: 2,
            }}
          >
            <EditIcon /> Edit Event
          </Typography>
        </Box>

        <CardContent sx={{ py: 4 }}>
          <form onSubmit={handleSubmit}>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Event Name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  variant="outlined"
                  required
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Description"
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  variant="outlined"
                  multiline
                  rows={4}
                  required
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormControl fullWidth variant="outlined">
                  <InputLabel>Category</InputLabel>
                  <Select
                    name="category"
                    value={formData.category}
                    onChange={handleInputChange}
                    label="Category"
                    required
                  >
                    {categories.map((category) => (
                      <MenuItem key={category} value={category}>
                        {category}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Available Tickets"
                  name="maxTicketCount"
                  value={formData.maxTicketCount}
                  onChange={handleInputChange}
                  type="number"
                  variant="outlined"
                  required
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DateTimePicker
                    label="Start Date"
                    value={formData.startDate}
                    onChange={handleDateChange("startDate")}
                    renderInput={(params) => (
                      <TextField {...params} fullWidth required />
                    )}
                  />
                </LocalizationProvider>
              </Grid>

              <Grid item xs={12} md={6}>
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DateTimePicker
                    label="End Date"
                    value={formData.endDate}
                    onChange={handleDateChange("endDate")}
                    minDate={formData.startDate}
                    renderInput={(params) => (
                      <TextField {...params} fullWidth required />
                    )}
                  />
                </LocalizationProvider>
              </Grid>

              <Grid item xs={12}>
                <Box
                  mt={3}
                  sx={{ borderTop: "1px solid", borderColor: "divider", pt: 3 }}
                >
                  <Stack direction="row" spacing={2} justifyContent="flex-end">
                    <Button variant="outlined" onClick={() => navigate(-1)}>
                      Cancel
                    </Button>
                    <Button
                      type="submit"
                      variant="contained"
                      color="primary"
                      disabled={loading}
                      startIcon={<SaveIcon />}
                    >
                      {loading ? "Saving..." : "Save Changes"}
                    </Button>
                  </Stack>
                </Box>
              </Grid>
            </Grid>
          </form>
        </CardContent>
      </Card>
      <Snackbar
        open={toast.open}
        autoHideDuration={3000}
        onClose={handleCloseToast}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <MuiAlert
          onClose={handleCloseToast}
          severity={toast.severity}
          sx={{ width: "100%" }}
          elevation={6}
          variant="filled"
        >
          {toast.message}
        </MuiAlert>
      </Snackbar>
    </Container>
  );
}

export default EventEdit;
