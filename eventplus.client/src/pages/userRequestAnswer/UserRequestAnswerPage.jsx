import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Container,
    Typography,
    Box,
    Divider,
    Card,
    CardContent,
    CircularProgress,
    Alert,
    Paper,
    Button,
    TextField,
    List, // Pridėta
    ListItem, // Pridėta
    ListItemText // Pridėta
} from '@mui/material';
import { Send as SendIcon } from '@mui/icons-material';
import { useAuth } from '../../context/AuthContext';
import { fetchQuestions } from '../../services/questionService';
import { createBulkUserRequestAnswers, fetchUserRequestAnswersByUserId } from '../../services/userRequestAnswerService';
import { useNotification } from '../../context/NotificationContext';

const UserRequestAnswerPage = () => {
    const [questions, setQuestions] = useState([]);
    const [answers, setAnswers] = useState({});
    const [submittedAnswers, setSubmittedAnswers] = useState([]); // Nauja būsena pateiktiems atsakymams
    const [loadingQuestions, setLoadingQuestions] = useState(true);
    const [loadingSubmittedAnswers, setLoadingSubmittedAnswers] = useState(true); // Atskira būsena pateiktiems atsakymams
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const { currentUser, isAuthenticated } = useAuth();
    const { showSuccess, showError } = useNotification();
    const navigate = useNavigate();

    const loadSubmittedAnswers = useCallback(async () => {
        if (currentUser?.id) {
            try {
                setLoadingSubmittedAnswers(true);
                const data = await fetchUserRequestAnswersByUserId(currentUser.id);
                setSubmittedAnswers(data);
            } catch (err) {
                console.error('Klaida kraunant pateiktus atsakymus:', err);
                // Nerodome klaidos pranešimo čia, kad neužgožtų klausimyno klaidų
            } finally {
                setLoadingSubmittedAnswers(false);
            }
        } else {
            setSubmittedAnswers([]);
            setLoadingSubmittedAnswers(false);
        }
    }, [currentUser?.id]);

    useEffect(() => {
        const loadQuestionsData = async () => {
            try {
                setLoadingQuestions(true);
                const data = await fetchQuestions();
                setQuestions(data);
                const initialAnswers = {};
                data.forEach(q => {
                    initialAnswers[q.idQuestion] = '';
                });
                setAnswers(initialAnswers);
                setError(null);
            } catch (err) {
                console.error('Klaida kraunant klausimus:', err);
                setError('Nepavyko gauti klausimų sąrašo. Bandykite vėliau.');
            } finally {
                setLoadingQuestions(false);
            }
        };

        if (isAuthenticated()) {
            loadQuestionsData();
            loadSubmittedAnswers(); // Įkrauname ir pateiktus atsakymus
        } else {
            setLoadingQuestions(false);
            setLoadingSubmittedAnswers(false);
            setError("Prisijunkite, kad galėtumėte peržiūrėti ir pildyti klausimyną.");
        }
    }, [isAuthenticated, loadSubmittedAnswers]);

    const handleAnswerChange = (questionId, value) => {
        setAnswers(prevAnswers => ({
            ...prevAnswers,
            [questionId]: value
        }));
    };

    const allQuestionsAnswered = () => {
        if (questions.length === 0) return false;
        return questions.every(q => answers[q.idQuestion] && answers[q.idQuestion].trim() !== '');
    };

    const handleSubmitAllAnswers = async () => {
        if (!allQuestionsAnswered()) {
            showError('Prašome atsakyti į visus klausimus.');
            return;
        }

        setIsSubmitting(true);
        const answersPayload = questions.map(q => ({
            Answer: answers[q.idQuestion],
            FkQuestionidQuestion: q.idQuestion,
            FkUseridUser: currentUser?.id // Nors backendas tai nustato, galime siųsti ir iš čia
        }));

        try {
            await createBulkUserRequestAnswers(answersPayload);
            showSuccess('Visi atsakymai sėkmingai pateikti!');
            // Atnaujiname pateiktų atsakymų sąrašą
            await loadSubmittedAnswers();
            // Išvalome formos laukus (nebūtina, jei vartotojas nukreipiamas)
            const initialAnswers = {};
            questions.forEach(q => {
                initialAnswers[q.idQuestion] = '';
            });
            setAnswers(initialAnswers);
            // navigate('/'); // Galima palikti nukreipimą arba leisti matyti atnaujintą sąrašą
        } catch (err) {
            showError(`Klaida pateikiant atsakymus: ${err.message || 'Nežinoma klaida'}`);
            console.error('Klaida siunčiant bulk atsakymus:', err);
        } finally {
            setIsSubmitting(false);
        }
    };

    if (loadingQuestions) {
        return (
            <Container maxWidth="lg" sx={{ mt: 12, mb: 8, display: 'flex', justifyContent: 'center' }}>
                <CircularProgress />
            </Container>
        );
    }
    
    if (error && !isAuthenticated()) {
         return (
            <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
                <Alert severity="warning">{error} <Button onClick={() => navigate('/login')}>Prisijungti</Button></Alert>
            </Container>
        );
    }
    
    if (error) { // Klaida kraunant klausimus
        return (
            <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
                <Alert severity="error">{error}</Alert>
            </Container>
        );
    }
    
    if (!isAuthenticated()) {
        return (
            <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
                <Alert severity="info">Turite būti prisijungęs, kad galėtumėte pildyti klausimyną ir matyti savo atsakymus.</Alert>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
            {/* Klausimyno forma */}
            <Box sx={{ mb: 4 }}>
                <Typography variant="h4" component="h1" sx={{ fontWeight: 700, mb: 1 }}>
                    Klausimynas
                </Typography>
                <Typography variant="subtitle1" color="text.secondary" sx={{ mb: 2 }}>
                    Atsakykite į visus klausimus ir pateikite.
                </Typography>
                <Divider />
            </Box>

            <Paper sx={{ p: 3, mb: 4 }}>
                {questions.length > 0 ? (
                    questions.map((question) => (
                        <Card key={question.idQuestion} sx={{ mb: 3, borderRadius: 2, boxShadow: '0 2px 8px rgba(0,0,0,0.05)' }}>
                            <CardContent>
                                <Typography variant="subtitle1" fontWeight={600} gutterBottom sx={{ mb: 1.5 }}>
                                    {question.formulatedQuestion}
                                </Typography>
                                <TextField
                                    fullWidth
                                    label="Jūsų atsakymas"
                                    multiline
                                    rows={3}
                                    value={answers[question.idQuestion] || ''}
                                    onChange={(e) => handleAnswerChange(question.idQuestion, e.target.value)}
                                    variant="outlined"
                                    disabled={isSubmitting}
                                />
                            </CardContent>
                        </Card>
                    ))
                ) : (
                    !loadingQuestions && <Alert severity="info">Šiuo metu nėra sukurtų klausimų.</Alert>
                )}

                {questions.length > 0 && (
                    <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end' }}>
                        <Button
                            variant="contained"
                            color="primary"
                            size="large"
                            onClick={handleSubmitAllAnswers}
                            disabled={!allQuestionsAnswered() || isSubmitting}
                            endIcon={isSubmitting ? <CircularProgress size={20} color="inherit" /> : <SendIcon />}
                        >
                            {isSubmitting ? 'Siunčiama...' : 'Pateikti visus atsakymus'}
                        </Button>
                    </Box>
                )}
            </Paper>

            {/* Pateiktų atsakymų sąrašas */}
            <Box sx={{ mt: 6, mb: 4 }}>
                <Typography variant="h5" component="h2" sx={{ fontWeight: 600, mb: 2 }}>
                    Jūsų pateikti atsakymai
                </Typography>
                <Divider sx={{ mb: 3 }}/>
                {loadingSubmittedAnswers ? (
                    <Box sx={{ display: 'flex', justifyContent: 'center' }}>
                        <CircularProgress />
                    </Box>
                ) : submittedAnswers.length === 0 ? (
                    <Alert severity="info">Jūs dar nepateikėte jokių atsakymų.</Alert>
                ) : (
                    <Paper elevation={1} sx={{ p: 0 }}>
                        <List disablePadding>
                            {submittedAnswers.map((sa, index) => (
                                <React.Fragment key={sa.idUserRequestAnswer}>
                                    <ListItem sx={{ py: 2, display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Klausimas:
                                        </Typography>
                                        <Typography variant="body1" sx={{ fontWeight: 500, mb: 1 }}>
                                            {sa.questionText || `Nežinomas klausimas (ID: ${sa.fkQuestionidQuestion})`}
                                        </Typography>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom sx={{mt:1}}>
                                            Jūsų atsakymas:
                                        </Typography>
                                        <Typography variant="body2" sx={{ pl: 1, borderLeft: '3px solid', borderColor: 'primary.main', fontStyle: 'italic' }}>
                                            {sa.answer}
                                        </Typography>
                                    </ListItem>
                                    {index < submittedAnswers.length - 1 && <Divider component="li" />}
                                </React.Fragment>
                            ))}
                        </List>
                    </Paper>
                )}
            </Box>
        </Container>
    );
};

export default UserRequestAnswerPage;