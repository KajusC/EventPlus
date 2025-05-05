import React from 'react';
import { Snackbar, Alert } from '@mui/material';

const ToastNotification = ({ 
  open, 
  message, 
  severity = 'info', 
  onClose, 
  autoHideDuration = 3000,
  position = { vertical: 'top', horizontal: 'center' }
}) => {
  return (
    <Snackbar
      open={open}
      autoHideDuration={autoHideDuration}
      onClose={onClose}
      anchorOrigin={position}
    >
      <Alert 
        onClose={onClose} 
        severity={severity} 
        sx={{ width: '100%' }}
        elevation={6}
        variant="filled"
      >
        {message}
      </Alert>
    </Snackbar>
  );
};

export default ToastNotification;