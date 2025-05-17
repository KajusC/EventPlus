import React, { useState, useEffect } from "react";
import ToastNotification from "../../components/shared/ToastNotification";
import { scanTicket } from "../../services/ticketService";
import { Html5QrcodeScanner } from "html5-qrcode";
import qrCode from "/qrcode.png";
// You may need to install and import a QR code scanner library, e.g. react-qr-reader
// import QrReader from 'react-qr-reader';

const SCAN_TIMEOUT = 5000; // 5 seconds timeout after successful scan

const TicketScanPage = () => {
	const [scanResult, setScanResult] = useState(null);
	const [notification, setNotification] = useState(null);
	const [loading, setLoading] = useState(false);
	const [scanner, setScanner] = useState(null);

	useEffect(() => {
		// Initialize scanner with improved configuration
		const newScanner = new Html5QrcodeScanner("reader", {
			qrbox: { width: 250, height: 250 }, // Smaller box for better focus
			fps: 10, // Higher frame rate
			aspectRatio: 1.0,
			showTorchButtonIfSupported: true,
			showZoomSliderIfSupported: true,
			defaultZoomValueIfSupported: 2,
			formatsToSupport: ["QR_CODE"], // Only look for QR codes
			rememberLastUsedCamera: true,
			showScanButton: true,
			showStopButton: true,
		});

		setScanner(newScanner);

		const handleScan = async (result) => {
			console.log("QR Code scanned:", result);
			setLoading(true);
			try {
				const response = await scanTicket(result);
				console.log("Scan response:", response);
				setScanResult(response);
				setNotification({
					type: response.isValid ? "success" : "error",
					message: response.message,
				});

				// Stop scanning after successful scan
				if (response.isValid) {
					handleStopScan();
					// Resume scanning after timeout
					setTimeout(() => {
						handleStartScan();
					}, SCAN_TIMEOUT);
				}
			} catch (error) {
				console.error("Scan error:", error);
				setNotification({
					type: "error",
					message: error.response?.data?.message || "Scan failed. Please try again.",
				});
			} finally {
				setLoading(false);
			}
		};

		const handleError = (err) => {
			// Only show error notification for non-NotFound errors
			if (!err?.includes("NotFoundException")) {
				console.error("Scanner error:", err);
				setNotification({
					type: "error",
					message: "Scanner error. Please try again.",
				});
			}
		};

		newScanner.render(handleScan, handleError);

		// Cleanup
		return () => {
			if (newScanner) {
				newScanner.clear();
			}
		};
	}, []);

	const handleStopScan = () => {
		if (scanner) {
			scanner.clear();
			setScanner(null);
		}
	};

	const handleStartScan = () => {
		if (!scanner) {
			const newScanner = new Html5QrcodeScanner("reader", {
				qrbox: { width: 250, height: 250 },
				fps: 10,
				aspectRatio: 1.0,
				showTorchButtonIfSupported: true,
				showZoomSliderIfSupported: true,
				defaultZoomValueIfSupported: 2,
				formatsToSupport: ["QR_CODE"],
				rememberLastUsedCamera: true,
				showScanButton: true,
				showStopButton: true,
			});

			setScanner(newScanner);
			newScanner.render(
				(result) => {
					console.log("QR Code scanned:", result);
					setLoading(true);
					scanTicket(result)
						.then((response) => {
							console.log("Scan response:", response);
							setScanResult(response);
							setNotification({
								type: response.isValid ? "success" : "error",
								message: response.message,
							});

							// Stop scanning after successful scan
							if (response.isValid) {
								handleStopScan();
								// Resume scanning after timeout
								setTimeout(() => {
									handleStartScan();
								}, SCAN_TIMEOUT);
							}
						})
						.catch((error) => {
							console.error("Scan error:", error);
							setNotification({
								type: "error",
								message: error.response?.data?.message || "Scan failed. Please try again.",
							});
						})
						.finally(() => {
							setLoading(false);
						});
				},
				(err) => {
					if (!err?.includes("NotFoundException")) {
						console.error("Scanner error:", err);
						setNotification({
							type: "error",
							message: "Scanner error. Please try again.",
						});
					}
				}
			);
		}
	};

	return (
		<div style={{ padding: 24, paddingTop: 100 }}>
			<h2>Scan Ticket QR Code</h2>
			
			<div style={{ marginBottom: 20 }}>
				<button 
					onClick={handleStartScan}
					style={{
						padding: "10px 20px",
						backgroundColor: "#4CAF50",
						color: "white",
						border: "none",
						borderRadius: "4px",
						cursor: "pointer",
						marginRight: "10px"
					}}
				>
					Start Scanner
				</button>
				<button 
					onClick={handleStopScan}
					style={{
						padding: "10px 20px",
						backgroundColor: "#f44336",
						color: "white",
						border: "none",
						borderRadius: "4px",
						cursor: "pointer"
					}}
				>
					Stop Scanner
				</button>
			</div>

			<div id="reader" style={{ width: "100%", maxWidth: "600px", margin: "0 auto" }}></div>
			
			{loading && (
				<div style={{ textAlign: "center", margin: "20px 0" }}>
					Processing scan...
				</div>
			)}

			{scanResult && (
				<div style={{ 
					marginTop: 16, 
					padding: "20px", 
					border: "1px solid #ccc", 
					borderRadius: "8px",
					backgroundColor: scanResult.isValid ? "#f0fff0" : "#fff0f0"
				}}>
					<div style={{ 
						display: "flex", 
						alignItems: "center", 
						marginBottom: "16px",
						padding: "10px",
						backgroundColor: scanResult.isValid ? "#4CAF50" : "#f44336",
						borderRadius: "4px",
						color: "white"
					}}>
						<h4 style={{ margin: 0 }}>{scanResult.isValid ? "✓ Valid Ticket" : "✗ Invalid Ticket"}</h4>
					</div>

					<div style={{ marginBottom: "16px", color: "black" }}>
						<strong>Status Message:</strong> {scanResult.message}
					</div>

					{scanResult.ticket && (
						<div style={{ 
							marginTop: "16px",
							backgroundColor: "white",
							padding: "16px",
							borderRadius: "4px",
							boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
						}}>
							<h5 style={{ 
								marginTop: 0,
								marginBottom: "16px",
								paddingBottom: "8px",
								borderBottom: "2px solid #eee",
								color: "black"
							}}>Ticket Details</h5>
							
							<div style={{ 
								display: "grid",
								gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
								gap: "16px",
								color: "black"
							}}>
								<div>
									<strong>Ticket ID:</strong>
									<div>{scanResult.ticket.idTicket}</div>
								</div>
								<div>
									<strong>Event ID:</strong>
									<div>{scanResult.ticket.fkEventidEvent}</div>
								</div>
								<div>
									<strong>Price:</strong>
									<div>${scanResult.ticket.price.toFixed(2)}</div>
								</div>
								<div>
									<strong>Generation Date:</strong>
									<div>{new Date(scanResult.ticket.generationDate).toLocaleDateString()}</div>
								</div>
								<div>
									<strong>Seating ID:</strong>
									<div>{scanResult.ticket.seatingId}</div>
								</div>
								<div>
									<strong>Ticket Status:</strong>
									<div>{scanResult.ticket.ticketStatusId}</div>
								</div>
								<div>
									<strong>Scanned Date:</strong>
									<div>
										{scanResult.ticket.scannedDate === "0001-01-01T00:00:00" 
											? "Not scanned yet" 
											: new Date(scanResult.ticket.scannedDate).toLocaleString()}
									</div>
								</div>
							</div>
						</div>
					)}
				</div>
			)}

			<div style={{ marginTop: 16, textAlign: "center" }}>
				<p>Test QR Code:</p>
				<img
					src={qrCode}
					alt="Test QR Code"
					style={{ 
						border: "1px solid #ccc", 
						width: "200px", 
						height: "200px",
						objectFit: "contain"
					}}
				/>
			</div>

			{notification && (
				<ToastNotification
					type={notification.type}
					message={notification.message}
					onClose={() => setNotification(null)}
				/>
			)}
		</div>
	);
};

export default TicketScanPage;
