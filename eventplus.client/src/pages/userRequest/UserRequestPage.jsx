import React, { useState, useEffect } from 'react';
import { 
    Container, 
    Typography, 
    Box, 
    Divider, 
    Grid, 
    Card, 
    CardContent,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    CircularProgress,
    Alert
} from '@mui/material';
import UserRequestAnswerForm from '../../components/userRequestAnswer/UserRequestAnswerForm';
import UserRequestAnswerList from '../../components/userRequestAnswer/UserRequestAnswerList';
import { useAuth } from '../../context/AuthContext';
import { fetchQuestions } from '../../services/questionService';

const UserRequestAnswersPage = () => {
    const [questions, setQuestions] = useState([]);
    const [selectedQuestionId, setSelectedQuestionId] = useState('');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { isAuthenticated } = useAuth();
    
    useEffect(() => {
        const loadQuestions = async () => {
            try {
                setLoading(true);
                const data = await fetchQuestions();
                setQuestions(data);
                
                if (data.length > 0) {
                    setSelectedQuestionId(data[0].idQuestion);
                }
                
                setError(null);
            } catch (err) {
                console.error('Klaida kraunant klausimus:', err);
                setError('Nepavyko gauti klausimų sąrašo. Bandykite vėliau.');
            } finally {
                setLoading(false);
            }
        };
        
        loadQuestions();
    }, []);
    
    const handleQuestionChange = (event) => {
        setSelectedQuestionId(event.target.value);
    };
    
    if (loading) {
        return (
            <Container maxWidth="lg" sx={{ mt: 12, mb: 8, display: 'flex', justifyContent: 'center' }}>
                <CircularProgress />
            </Container>
        );
    }
    
    if (error) {
        return (
            <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
                <Alert severity="error">{error}</Alert>
            </Container>
        );
    }
    
    return (
        <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
            <Box sx={{ mb: 4 }}>
                <Typography variant="h4" component="h1" sx={{ fontWeight: 700, mb: 1 }}>
                    Naudotojo užklausos
                </Typography>
                <Typography variant="subtitle1" color="text.secondary" sx={{ mb: 2 }}>
                    Pateikite savo užklausas ir peržiūrėkite ankstesnius atsakymus
                </Typography>
                <Divider />
            </Box>
            
            <Grid container spacing={4}>
                {/* Užklausų forma */}
                <Grid item xs={12} md={5}>
                    <Card sx={{ borderRadius: 2, boxShadow: '0 5px 15px rgba(0,0,0,0.08)' }}>
                        <CardContent sx={{ p: 3 }}>
                            <Typography variant="h5" component="h2" sx={{ fontWeight: 600, mb: 3 }}>
                                Nauja užklausa
                            </Typography>
                            
                            <FormControl fullWidth sx={{ mb: 3 }}>
                                <InputLabel id="question-select-label">Pasirinkite klausimą</InputLabel>
                                <Select
                                    labelId="question-select-label"
                                    id="question-select"
                                    value={selectedQuestionId}
                                    label="Pasirinkite klausimą"
                                    onChange={handleQuestionChange}
                                >
                                    {questions.map(question => (
                                        <MenuItem key={question.idQuestion} value={question.idQuestion}>
                                            {question.formulatedQuestion}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            
                            {selectedQuestionId && isAuthenticated() && (
                                <UserRequestAnswerForm 
                                    questionId={selectedQuestionId} 
                                    onRequestSubmitted={() => {}}
                                />
                            )}
                            
                            {!isAuthenticated() && (
                                <Typography variant="body1" color="error" sx={{ mt: 2 }}>
                                    Prisijunkite, kad galėtumėte pateikti užklausą.
                                </Typography>
                            )}
                        </CardContent>
                    </Card>
                </Grid>
                
                {/* Užklausų sąrašas */}
                <Grid item xs={12} md={7}>
                    <Box sx={{ mb: 3 }}>
                        <Typography variant="h5" component="h2" sx={{ fontWeight: 600 }}>
                            Jūsų užklausos
                        </Typography>
                    </Box>
                    
                    {isAuthenticated() ? (
                        <UserRequestAnswerList />
                    ) : (
                        <Typography variant="body1" color="error">
                            Prisijunkite, kad galėtumėte peržiūrėti savo užklausas.
                        </Typography>
                    )}
                </Grid>
            </Grid>
        </Container>
    );
};

export default UserRequestAnswersPage;