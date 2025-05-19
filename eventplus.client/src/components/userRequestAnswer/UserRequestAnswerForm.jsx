import React, { useState } from 'react';
import { 
    TextField, 
    Button, 
    Box, 
    FormControl, 
    FormHelperText,
    CircularProgress
} from '@mui/material';
import { Send as SendIcon } from '@mui/icons-material';
import { useAuth } from '../../context/AuthContext';
import { createUserRequestAnswer } from '../../services/userRequestAnswerService';
import { useNotification } from '../../context/NotificationContext';

const UserRequestAnswerForm = ({ questionId, onRequestSubmitted }) => {
    const [answer, setAnswer] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { currentUser } = useAuth();
    const { showSuccess, showError } = useNotification();

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!answer.trim()) {
            showError('Atsakymas negali būti tuščias');
            return;
        }
        
        setIsSubmitting(true);
        
        try {
            const requestData = {
                IdUserRequestAnswer: 0,
                Answer: answer,
                FkQuestionidQuestion: questionId,
                FkUseridUser: currentUser?.id
            };
            
            await createUserRequestAnswer(requestData);
            showSuccess('Jūsų atsakymas sėkmingai išsiųstas!');
            setAnswer('');
            
            if (onRequestSubmitted) {
                onRequestSubmitted();
            }
        } catch (error) {
            showError(`Klaida siunčiant atsakymą: ${error.message}`);
        } finally {
            setIsSubmitting(false);
        }
    };
    
    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
            <FormControl fullWidth sx={{ mb: 2 }}>
                <TextField
                    label="Jūsų atsakymas"
                    multiline
                    rows={3}
                    value={answer}
                    onChange={(e) => setAnswer(e.target.value)}
                    variant="outlined"
                    required
                    disabled={isSubmitting}
                />
                <FormHelperText>
                    Pateikite savo atsakymą į klausimą
                </FormHelperText>
            </FormControl>
            
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                <Button 
                    type="submit" 
                    variant="contained" 
                    color="primary"
                    disabled={isSubmitting} 
                    endIcon={isSubmitting ? <CircularProgress size={16} /> : <SendIcon />}
                    sx={{ 
                        py: 1,
                        px: 3,
                        fontWeight: 600
                    }}
                >
                    {isSubmitting ? 'Siunčiama...' : 'Pateikti'}
                </Button>
            </Box>
        </Box>
    );
};

export default UserRequestAnswerForm;